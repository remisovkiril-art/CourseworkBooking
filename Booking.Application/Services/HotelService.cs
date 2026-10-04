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

        var result = ApplyBaseFilters(hotels, request);

        // Minimum rating
        if (request.MinRating.HasValue)
        {
            result = result.Where(h =>
                h.Reviews.Any() &&
                h.Reviews.Average(r => r.Rating) >= request.MinRating.Value);
        }

        // Stars
        if (request.Stars.HasValue)
        {
            result = result.Where(h =>
                h.Stars >= request.Stars.Value);
        }

        //  Hotel types
        if (request.Types != null && request.Types.Length > 0)
        {
            result = result.Where(h =>
                request.Types.Contains(h.Type));
        }

        // Hotel chains
        if (request.ChainIds != null && request.ChainIds.Length > 0)
        {
            result = result.Where(h =>
                h.HotelChainId.HasValue &&
                request.ChainIds.Contains(h.HotelChainId.Value));
        }

        // Amenities
        if (request.Amenities != null && request.Amenities.Length > 0)
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

        // Sorting
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

        // Total до pagination
        var total = result.Count();

        // Pagination
        var page = Math.Max(request.Page, 1);

        var pageSize = request.PageSize <= 0
            ? 20
            : request.PageSize;

        var hotelsResult = result
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(Map)
            .ToList();

        // Return
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
        var hotels = await _hotelRepository.GetAllAsync(cancellationToken);


        var result = ApplyBaseFilters(hotels, request);

        var hotelsList = result.ToList();

        // Rating

        var ratings = new List<RatingFilterDto>
    {
        new()
        {
            MinRating = 9,
            Count = hotelsList.Count(h =>
                h.Reviews.Any() &&
                h.Reviews.Average(r => r.Rating) >= 9)
        },

        new()
        {
            MinRating = 8,
            Count = hotelsList.Count(h =>
                h.Reviews.Any() &&
                h.Reviews.Average(r => r.Rating) >= 8)
        },

        new()
        {
            MinRating = 7,
            Count = hotelsList.Count(h =>
                h.Reviews.Any() &&
                h.Reviews.Average(r => r.Rating) >= 7)
        },

        new()
        {
            MinRating = 6,
            Count = hotelsList.Count(h =>
                h.Reviews.Any() &&
                h.Reviews.Average(r => r.Rating) >= 6)
        }
    };

       
        // Stars

        var stars = hotelsList
            .GroupBy(h => h.Stars)
            .OrderByDescending(g => g.Key)
            .Select(g => new StarsFilterDto
            {
                Stars = g.Key,
                Count = g.Count()
            })
            .ToList();

        // Types

        var types = hotelsList
            .GroupBy(h => h.Type)
            .Select(g => new HotelTypeFilterDto
            {
                Type = g.Key.ToString(),
                Count = g.Count()
            })
            .ToList();

        // Chains

        var chains = hotelsList
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
            .ToList();

        // Amenities

        var amenities = hotelsList
            .SelectMany(h => h.Amenities)
            .GroupBy(a => a.AmenityName)
            .Select(g => new AmenityFilterDto
            {
                Name = g.Key,
                Count = g.Select(a => a.HotelId).Distinct().Count()
            })
            .OrderByDescending(x => x.Count)
            .ToList();

        return new HotelFiltersDto
        {
            Ratings = ratings,
            Stars = stars,
            Types = types,
            Chains = chains,
            Amenities = amenities
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

