using AutoMapper;
using Booking.Application.DTOs.Hotels;
using Booking.Application.DTOs.Reviews;
using Booking.Application.Interfaces.Repository;
using Booking.Application.Interfaces.Services;
using Booking.Domain.Entities;

namespace Booking.Application.Services;

public class HotelService : IHotelService
{
    private readonly IHotelRepository _hotelRepository;
    private readonly IMapper _mapper;
    private readonly ICachingService _cacheService;

    public HotelService(
        IHotelRepository hotelRepository,
        IMapper mapper,
        ICachingService cacheService)
    {
        _hotelRepository = hotelRepository;
        _mapper = mapper;
        _cacheService = cacheService;
    }

    public async Task<HotelSearchResultDto> SearchAsync(
        HotelSearchDto dto,
        CancellationToken cancellationToken)
    {
        List<Hotel> hotels =
            await _hotelRepository.SearchAsync(
                dto,
                cancellationToken);

        List<HotelDto> result =
            hotels.Select(Map).ToList();

        return new HotelSearchResultDto
        {
            TotalCount = result.Count,
            Hotels = result,
        };
    }

    public async Task<HotelDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        string cacheKey =
            $"hotel:{id}";

        HotelDto? cached =
            await _cacheService.GetAsync<HotelDto>(
                cacheKey);

        if (cached != null)
        {
            return cached;
        }

        Hotel? hotel =
            await _hotelRepository.GetByIdAsync(
                id,
                cancellationToken);

        if (hotel == null)
        {
            return null;
        }

        HotelDto result =
            Map(hotel);

        await _cacheService.SetAsync(
            cacheKey,
            result,
            TimeSpan.FromMinutes(15));

        return result;
    }

    public async Task<HotelDto> CreateAsync(
        HotelCreateDto dto,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            throw new Exception(
                "Hotel name is required");
        }

        if (dto.Stars < 1 ||
            dto.Stars > 5)
        {
            throw new Exception(
                "Hotel stars must be from 1 to 5");
        }

        if (dto.Rooms.Count < 1 ||
            dto.Rooms.Count > 4)
        {
            throw new Exception(
                "A hotel must have from 1 to 4 rooms");
        }

        Hotel hotel =
            _mapper.Map<Hotel>(dto);

        hotel.Id =
            Guid.NewGuid();

        foreach (string amenity in dto.Amenities)
        {
            if (string.IsNullOrWhiteSpace(amenity))
            {
                continue;
            }

            hotel.Amenities.Add(
                new HotelAmenity
                {
                    Id = Guid.NewGuid(),
                    HotelId = hotel.Id,
                    AmenityName =
                        amenity.Trim()
                });
        }

        if (dto.HasWifi == true &&
            !hotel.Amenities.Any(x =>
                x.AmenityName.Equals(
                    "Wi-Fi",
                    StringComparison.OrdinalIgnoreCase)))
        {
            hotel.Amenities.Add(
                new HotelAmenity
                {
                    Id = Guid.NewGuid(),
                    HotelId = hotel.Id,
                    AmenityName = "Wi-Fi"
                });
        }

        foreach (RoomCreateDto roomDto in dto.Rooms)
        {
            Room room =
                _mapper.Map<Room>(roomDto);

            room.Id =
                Guid.NewGuid();

            room.HotelId =
                hotel.Id;

            hotel.Rooms.Add(room);
        }

        await _hotelRepository.AddAsync(
            hotel,
            cancellationToken);

        return Map(hotel);
    }

    public async Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        await _hotelRepository.DeleteAsync(
            id,
            cancellationToken);

        await _cacheService.RemoveAsync(
            $"hotel:{id}");
    }

    private static HotelDto Map(
        Hotel hotel)
    {
        List<ReviewDto> reviews =
            hotel.Reviews
                .OrderByDescending(x =>
                    x.CreatedAt)
                .Select(x =>
                    new ReviewDto
                    {
                        Id = x.Id,
                        HotelId = x.HotelId,
                        AuthorName =
                            x.User.Name ??
                            "User",
                        AuthorAvatarUrl =
                            x.User.AvatarUrl,
                        Text = x.Comment,
                        Rating = x.Rating,
                        CreatedAt =
                            x.CreatedAt,
                        Facilities =
                            x.Facilities,
                        Staff = x.Staff,
                        Cleanliness =
                            x.Cleanliness,
                        Comfort =
                            x.Comfort,
                        Location =
                            x.Location,
                        ValueForMoney =
                            x.ValueForMoney
                    })
                .ToList();

        bool hasReviews =
            hotel.Reviews.Count > 0;

        double rating =
            hasReviews
                ? Math.Round(
                    hotel.Reviews.Average(
                        x => x.Rating),
                    1)
                : 0;

        double facilities =
            hasReviews
                ? Math.Round(
                    hotel.Reviews.Average(
                        x => x.Facilities),
                    1)
                : 0;

        double staff =
            hasReviews
                ? Math.Round(
                    hotel.Reviews.Average(
                        x => x.Staff),
                    1)
                : 0;

        double cleanliness =
            hasReviews
                ? Math.Round(
                    hotel.Reviews.Average(
                        x => x.Cleanliness),
                    1)
                : 0;

        double comfort =
            hasReviews
                ? Math.Round(
                    hotel.Reviews.Average(
                        x => x.Comfort),
                    1)
                : 0;

        double location =
            hasReviews
                ? Math.Round(
                    hotel.Reviews.Average(
                        x => x.Location),
                    1)
                : 0;

        double valueForMoney =
            hasReviews
                ? Math.Round(
                    hotel.Reviews.Average(
                        x => x.ValueForMoney),
                    1)
                : 0;

        List<string> images =
            hotel.Images
                .Select(x => x.ImageUrl)
                .ToList();

        bool? hasWifi =
            hotel.Amenities.Any(x =>
                x.AmenityName.Equals(
                    "Wi-Fi",
                    StringComparison.OrdinalIgnoreCase))
                ? true
                : null;

        string mapUrl =
            hotel.Latitude != 0 &&
            hotel.Longitude != 0
                ? $"https://www.google.com/maps?q={hotel.Latitude},{hotel.Longitude}"
                : string.Empty;

        return new HotelDto
        {
            Id = hotel.Id,
            Name = hotel.Name,
            Address = hotel.Address,
            City = hotel.City,
            Country = hotel.Country,
            Description = hotel.Description,
            Stars = hotel.Stars,
            HotelType = hotel.HotelType,

            HotelChain = hotel.HotelChain,
            Attractions = hotel.Attractions,

            Latitude = hotel.Latitude,
            Longitude = hotel.Longitude,
            MapUrl = mapUrl,

            IsPopular = hotel.IsPopular,
            IsCityCentre =
                hotel.IsCityCentre,
            IsPopularPlace =
                hotel.IsPopularPlace,

            NearMetro =
                hotel.NearMetro,
            NearAirport =
                hotel.NearAirport,
            NearStation =
                hotel.NearStation,

            Rating = rating,
            Facilities = facilities,
            Staff = staff,
            Cleanliness = cleanliness,
            Comfort = comfort,
            Location = location,
            ValueForMoney = valueForMoney,

            ReviewsCount =
                reviews.Count,

            MainImageUrl =
                images.FirstOrDefault() ??
                string.Empty,

            Images = images,

            Amenities =
                hotel.Amenities
                    .Select(x =>
                        x.AmenityName)
                    .ToList(),

            HasWifi = hasWifi,

            Rooms =
                hotel.Rooms
                    .Select(x =>
                        new RoomDto
                        {
                            Id = x.Id,
                            HotelId =
                                x.HotelId,
                            Title =
                                x.Title,
                            BedType =
                                x.BedType,
                            Capacity =
                                x.Capacity,
                            PricePerNight =
                                x.PricePerNight,
                            IsAvailable =
                                x.IsAvailable,
                            ImageUrl =
                                x.ImageUrl
                        })
                    .ToList(),

            Reviews = reviews
        };
    }
}