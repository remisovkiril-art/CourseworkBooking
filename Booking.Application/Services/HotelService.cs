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

    public HotelService(IHotelRepository hotelRepository, IMapper mapper, ICachingService cacheService)
    {
        _hotelRepository = hotelRepository;
        _mapper = mapper;
        _cacheService = cacheService;
    }

    public async Task<List<HotelDto>> GetAllAsync(
        string? search,
        int adults,
        int children,
        int rooms,
        DateTime? checkIn,
        DateTime? checkOut,
        CancellationToken cancellationToken)
    {
        var hotels = await _hotelRepository.GetAllAsync(cancellationToken);
        var result = hotels.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            result = result.Where(x =>
                x.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                x.City.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                x.Country.Contains(search, StringComparison.OrdinalIgnoreCase));
        }

        var people = adults + children;

        if (checkIn.HasValue && checkOut.HasValue && checkOut.Value > checkIn.Value)
        {
            result = result.Where(h => h.Rooms.Any(r =>
                r.IsAvailable &&
                (people == 0 || r.Capacity >= people) &&
                !r.Bookings.Any(b =>
                    b.Status != "Cancelled" &&
                    b.CheckInDate < checkOut.Value &&
                    b.CheckOutDate > checkIn.Value)));
        }
        else if (people > 0)
        {
            result = result.Where(h => h.Rooms.Any(r =>
                r.IsAvailable && r.Capacity >= people));
        }

        if (rooms > 0)
        {
            if (checkIn.HasValue && checkOut.HasValue && checkOut.Value > checkIn.Value)
            {
                result = result.Where(h =>
                    h.Rooms.Count(r =>
                        r.IsAvailable &&
                        !r.Bookings.Any(b =>
                            b.Status != "Cancelled" &&
                            b.CheckInDate < checkOut.Value &&
                            b.CheckOutDate > checkIn.Value)) >= rooms);
            }
            else
            {
                result = result.Where(h => h.Rooms.Count(r => r.IsAvailable) >= rooms);
            }
        }

        return result.Select(Map).ToList();
    }

    public async Task<HotelDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var cacheKey = $"hotel:{id}";

        var cached = await _cacheService.GetAsync<HotelDto>(cacheKey);

        if (cached != null)
            return cached;

        var hotel = await _hotelRepository.GetByIdAsync(
            id,
            cancellationToken);

        if (hotel == null)
            return null;

        var result = Map(hotel);

        await _cacheService.SetAsync(
            cacheKey,
            result,
            null);

        return result;
    }

    public async Task<HotelDto> CreateAsync(
        HotelCreateDto dto,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new Exception("Hotel name is required");

        if (dto.Rooms.Count < 1 || dto.Rooms.Count > 4)
            throw new Exception("A hotel must have from 1 to 4 rooms");

        var hotel = _mapper.Map<Hotel>(dto);
        hotel.Id = Guid.NewGuid();

        foreach (var amenity in dto.Amenities)
        {
            if (!string.IsNullOrWhiteSpace(amenity))
            {
                hotel.Amenities.Add(new HotelAmenity
                {
                    Id = Guid.NewGuid(),
                    HotelId = hotel.Id,
                    AmenityName = amenity.Trim()
                });
            }
        }

        if (dto.HasWifi == true &&
            !hotel.Amenities.Any(x =>
                x.AmenityName.Equals("Wi-Fi", StringComparison.OrdinalIgnoreCase)))
        {
            hotel.Amenities.Add(new HotelAmenity
            {
                Id = Guid.NewGuid(),
                HotelId = hotel.Id,
                AmenityName = "Wi-Fi"
            });
        }

        foreach (var roomDto in dto.Rooms)
        {
            var room = _mapper.Map<Room>(roomDto);
            room.Id = Guid.NewGuid();
            room.HotelId = hotel.Id;
            hotel.Rooms.Add(room);
        }

        await _hotelRepository.AddAsync(hotel, cancellationToken);
        return Map(hotel);
    }

    private static HotelDto Map(Hotel hotel)
    {
        var reviews = hotel.Reviews
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new ReviewDto
            {
                Id = x.Id,
                HotelId = x.HotelId,
                AuthorName = x.User.Name,
                Text = x.Comment,
                Rating = x.Rating,
                CreatedAt = x.CreatedAt
            })
            .ToList();

        var rating = hotel.Reviews.Count == 0
            ? 0
            : Math.Round(hotel.Reviews.Average(x => x.Rating), 1);

        var images = hotel.Images
            .Select(x => x.ImageUrl)
            .ToList();

        var hasWifi = hotel.Amenities.Any(x =>
            x.AmenityName.Equals("Wi-Fi", StringComparison.OrdinalIgnoreCase))
            ? true
            : (bool?)null;

        return new HotelDto
        {
            Id = hotel.Id,
            Name = hotel.Name,
            City = hotel.City,
            Country = hotel.Country,
            Description = hotel.Description,
            Rating = rating,
            ReviewsCount = reviews.Count,
            MainImageUrl = images.FirstOrDefault() ?? string.Empty,
            Images = images,
            Amenities = hotel.Amenities.Select(x => x.AmenityName).ToList(),
            HasWifi = hasWifi,
            Rooms = hotel.Rooms.Select(x => new RoomDto
            {
                Id = x.Id,
                HotelId = x.HotelId,
                Title = x.Title,
                BedType = x.BedType,
                Capacity = x.Capacity,
                PricePerNight = x.PricePerNight,
                IsAvailable = x.IsAvailable,
                ImageUrl = x.ImageUrl
            }).ToList(),
            Reviews = reviews
        };
    }
}
