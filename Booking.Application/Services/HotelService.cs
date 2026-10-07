using AutoMapper;
using Booking.Application.DTOs.Hotels;
using Booking.Application.DTOs.Reviews;
using Booking.Application.DTOs.Rooms;
using Booking.Application.DTOs.Stars;
using Booking.Application.Interfaces.Repository;
using Booking.Application.Interfaces.Services;
using Booking.Domain.Entities;
using Booking.Domain.Enum;

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

    public async Task<HotelSearchResultDto> GetAllAsync(
     HotelSearchDto request,
     CancellationToken cancellationToken)
    {
        var hotels = await _hotelRepository.GetAllAsync(cancellationToken);

        var result = ApplyFilters(hotels, request);

        result = request.Sort?.ToLower() switch
        {
            "rating" =>
                result.OrderByDescending(h =>
                    h.Reviews.Any()
                        ? h.Reviews.Average(r => r.Rating)
                        : 0),

            "price-asc" =>
                result.OrderBy(h =>
                    h.Rooms.Any()
                        ? h.Rooms.Min(r => r.PricePerNight)
                        : decimal.MaxValue),

            "price-desc" =>
                result.OrderByDescending(h =>
                    h.Rooms.Any()
                        ? h.Rooms.Min(r => r.PricePerNight)
                        : 0),

            _ => result
        };

        var total = result.Count();

        var page = Math.Max(request.Page, 1);

        var pageSize = request.PageSize <= 0
            ? 7
            : request.PageSize;

        var hotelsResult = result
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(Map)
            .ToList();

        return new HotelSearchResultDto
        {
            Hotels = hotelsResult,
            Total = total,
            Page = page,
            PageSize = pageSize
        };
    }



    public async Task<HotelFiltersDto> GetFiltersAsync(
        HotelSearchDto request,
        CancellationToken cancellationToken)
    {
        var hotels = await _hotelRepository.GetAllAsync(
            cancellationToken);

        var allHotels = hotels;

        var prices = allHotels
            .SelectMany(h => h.Rooms)
            .Select(r => r.PricePerNight)
            .ToList();

        var minPrice = prices.Any()
            ? prices.Min()
            : 0;

        var maxPrice = prices.Any()
            ? prices.Max()
            : 0;


        var result = ApplyFilters(
            allHotels,
            request);

        var hotelsList = result.ToList();


        // Rating counts:
        // враховуємо всі фільтри, крім MinRating
        var ratingHotels = ApplyFilters(
            hotels,
            request,
            applyRating: false
        ).ToList();

        // Stars counts:
        // враховуємо всі фільтри, крім Stars
        var starsHotels = ApplyFilters(
            hotels,
            request,
            applyStars: false
        ).ToList();

        // Types counts:
        // враховуємо всі фільтри, крім Types
        var typeHotels = ApplyFilters(
            hotels,
            request,
            applyTypes: false
        ).ToList();

        // Chains counts:
        // враховуємо всі фільтри, крім ChainIds
        var chainHotels = ApplyFilters(
            hotels,
            request,
            applyChains: false
        ).ToList();

        // Amenities counts:
        // враховуємо всі фільтри, крім Amenities
        var amenityHotels = ApplyFilters(
            hotels,
            request,
            applyAmenities: false
        ).ToList();



        var ratings = new List<RatingFilterDto>
        {
            new()
            {
                MinRating = 9,
                Count = ratingHotels.Count(h =>
                    h.Reviews.Any() &&
                    h.Reviews.Average(r => r.Rating) >= 9)
            },

            new()
            {
                MinRating = 8,
                Count = ratingHotels.Count(h =>
                    h.Reviews.Any() &&
                    h.Reviews.Average(r => r.Rating) >= 8)
            },

            new()
            {
                MinRating = 7,
                Count = ratingHotels.Count(h =>
                    h.Reviews.Any() &&
                    h.Reviews.Average(r => r.Rating) >= 7)
            },

            new()
            {
                MinRating = 6,
                Count = ratingHotels.Count(h =>
                    h.Reviews.Any() &&
                    h.Reviews.Average(r => r.Rating) >= 6)
            }
        };


        var stars = starsHotels
            .GroupBy(h => h.Stars)
            .OrderByDescending(g => g.Key)
            .Select(g => new StarsFilterDto
            {
                Stars = g.Key,
                Count = g.Count()
            })
            .ToList();


        var types = typeHotels
            .GroupBy(h => h.Type)
            .Select(g => new HotelTypeFilterDto
            {
                Type = g.Key.ToString(),
                Count = g.Count()
            })
            .ToList();


        var chains = chainHotels
            .Where(h => h.HotelChain != null)
            .GroupBy(h => new
            {
                h.HotelChainId,
                h.HotelChain!.Name
            })
            .Select(g => new ChainFilterDto
            {
                Id = g.Key.HotelChainId!.Value,
                Name = g.Key.Name,
                Count = g.Count()
            })
            .OrderByDescending(x => x.Count)
            .ToList();


        var amenities = amenityHotels
            .SelectMany(h => h.Amenities)
            .GroupBy(a => a.AmenityName)
            .Select(g => new AmenityFilterDto
            {
                Name = g.Key,
                Count = g.Select(a => a.HotelId)
                    .Distinct()
                    .Count()
            })
            .OrderByDescending(x => x.Count)
            .ToList();

        return new HotelFiltersDto
        {
            Ratings = ratings,
            Stars = stars,
            Types = types,
            Chains = chains,
            Amenities = amenities,
            MinPrice = minPrice,
            MaxPrice = maxPrice
        };
    }

    public async Task<HotelDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var cacheKey = $"hotel:{id}";

        var cached = await _cacheService.GetAsync<HotelDto>(
            cacheKey);

        if (cached != null)
        {
            return cached;
        }

        var hotel = await _hotelRepository.GetByIdAsync(
            id,
            cancellationToken);

        if (hotel == null)
        {
            return null;
        }

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
        {
            throw new Exception("Hotel name is required");
        }

        if (dto.Rooms.Count < 1 || dto.Rooms.Count > 4)
        {
            throw new Exception("A hotel must have from 1 to 4 rooms");
        }

        var hotel = new Hotel
        {
            Id = Guid.NewGuid(),
            Address = dto.Address,
            Name = dto.Name,
            City = dto.City,
            Country = dto.Country,
            Description = dto.Description,
            Stars = dto.Stars,
            Type = dto.Type
        };

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
                x.AmenityName.Equals(
                    "Wi-Fi",
                    StringComparison.OrdinalIgnoreCase)))
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

        await _hotelRepository.AddAsync(
            hotel,
            cancellationToken);

        return Map(hotel);
    }


    private IEnumerable<Hotel> ApplyBaseFilters(
        IEnumerable<Hotel> hotels,
        HotelSearchDto request)
    {
        var result = hotels;

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            result = result.Where(h =>
                h.Name.Contains(request.Search, StringComparison.OrdinalIgnoreCase) ||
                h.City.Contains(request.Search, StringComparison.OrdinalIgnoreCase) ||
                h.Country.Contains(request.Search, StringComparison.OrdinalIgnoreCase));
        }

        var people = request.Adults + request.Children;

        if (request.CheckIn.HasValue &&
            request.CheckOut.HasValue &&
            request.CheckOut > request.CheckIn)
        {
            result = result.Where(h =>
                h.Rooms.Any(r =>
                    r.IsAvailable &&
                    (people == 0 || r.Capacity >= people) &&
                    !r.Bookings.Any(b =>
                        b.Status != "Cancelled" &&
                        b.CheckInDate < request.CheckOut &&
                        b.CheckOutDate > request.CheckIn)));
        }
        else if (people > 0)
        {
            result = result.Where(h =>
                h.Rooms.Any(r =>
                    r.IsAvailable &&
                    r.Capacity >= people));
        }

        if (request.Rooms > 0)
        {
            result = result.Where(h =>
                h.Rooms.Count(r =>
                    r.IsAvailable &&
                    (
                        !request.CheckIn.HasValue ||
                        !request.CheckOut.HasValue ||
                        !r.Bookings.Any(b =>
                            b.Status != "Cancelled" &&
                            b.CheckInDate < request.CheckOut &&
                            b.CheckOutDate > request.CheckIn)
                    )
                ) >= request.Rooms);
        }

        return result;
    }

    private HotelDto Map(Hotel hotel)
    {
        var hasReviews = hotel.Reviews.Any();

        return new HotelDto
        {
            Id = hotel.Id,

            Name = hotel.Name,

            Address = hotel.Address,

            City = hotel.City,

            Country = hotel.Country,

            Description = hotel.Description,

            Type = hotel.Type,

            Stars = hotel.Stars,

            Rating = hasReviews
                ? hotel.Reviews.Average(r => r.Rating)
                : 0,

            ReviewsCount = hotel.Reviews.Count,

            Facilities = hasReviews
                ? hotel.Reviews.Average(r => r.Facilities)
                : 0,

            Staff = hasReviews
                ? hotel.Reviews.Average(r => r.Staff)
                : 0,

            Cleanliness = hasReviews
                ? hotel.Reviews.Average(r => r.Cleanliness)
                : 0,

            Comfort = hasReviews
                ? hotel.Reviews.Average(r => r.Comfort)
                : 0,

            Location = hasReviews
                ? hotel.Reviews.Average(r => r.Location)
                : 0,

            ValueForMoney = hasReviews
                ? hotel.Reviews.Average(r => r.ValueForMoney)
                : 0,

            MainImageUrl = hotel.Images
                .FirstOrDefault()?.ImageUrl ?? string.Empty,

            Images = hotel.Images
                .Select(i => i.ImageUrl)
                .ToList(),

            Amenities = hotel.Amenities
                .Select(a => a.AmenityName)
                .ToList(),

            HasWifi = hotel.Amenities.Any(a =>
                a.AmenityName.Equals(
                    "Wi-Fi",
                    StringComparison.OrdinalIgnoreCase)),

            Rooms = _mapper.Map<List<RoomDto>>(hotel.Rooms),

            Reviews = _mapper.Map<List<ReviewDto>>(hotel.Reviews)
        };
    }

    private IEnumerable<Hotel> ApplyFilters(
            IEnumerable<Hotel> hotels,
            HotelSearchDto request,
            bool applyRating = true,
            bool applyStars = true,
            bool applyTypes = true,
            bool applyChains = true,
            bool applyAmenities = true)
    {
        var result = ApplyBaseFilters(hotels, request);

        if (applyRating && request.MinRating.HasValue)
        {
            result = result.Where(h =>
                h.Reviews.Any() &&
                h.Reviews.Average(r => r.Rating)
                    >= request.MinRating.Value);
        }

        if (applyStars && request.Stars.HasValue)
        {
            result = result.Where(h => h.Stars == request.Stars.Value);
        }

        if (applyTypes &&
            request.Types != null &&
            request.Types.Length > 0)
        {
            result = result.Where(h =>
                request.Types.Contains(h.Type));
        }

        if (applyChains &&
            request.ChainIds != null &&
            request.ChainIds.Length > 0)
        {
            result = result.Where(h =>
                h.HotelChainId.HasValue &&
                request.ChainIds.Contains(h.HotelChainId.Value));
        }

        if (applyAmenities &&
            request.Amenities != null &&
            request.Amenities.Length > 0)
        {
            foreach (var amenity in request.Amenities)
            {
                result = result.Where(h =>
                    h.Amenities.Any(a =>
                        a.AmenityName.Equals(
                            amenity,
                            StringComparison.OrdinalIgnoreCase)));
            }
        }


        if (request.MinPrice.HasValue ||
        request.MaxPrice.HasValue)
        {
            result = result.Where(h =>
                h.Rooms.Any(r =>
                    (!request.MinPrice.HasValue ||
                     r.PricePerNight >= request.MinPrice.Value)
                    &&
                    (!request.MaxPrice.HasValue ||
                     r.PricePerNight <= request.MaxPrice.Value)
                ));
        }

        return result;
    }


    public async Task<List<HotelDto>?> GetRandomAsync(int number, CancellationToken cancellationToken)
    {
        var cacheKey = $"hotels:random:{number}";

        var cached = await _cacheService.GetAsync<List<HotelDto>>(
            cacheKey);

        if (cached != null)
        {
            return cached;
        }

        var hotels = await _hotelRepository.GetRandomHotelsAsync(
            number,
            cancellationToken);

        var result = hotels
        .Select(Map)
        .ToList();

        await _cacheService.SetAsync(
            cacheKey,
            result,
            null);

        return result;
    }
}

