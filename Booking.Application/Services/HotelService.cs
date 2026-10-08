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
        List<Hotel> hotels =
            await _hotelRepository.GetAllAsync(cancellationToken);

        return BuildSearchResult(hotels, request);
    }

    public async Task<HotelSearchResultDto> SearchAsync(
        HotelSearchDto dto,
        CancellationToken cancellationToken)
    {
        List<Hotel> hotels =
            await _hotelRepository.SearchAsync(
                dto,
                cancellationToken);

        return BuildSearchResult(hotels, dto);
    }

    public async Task<HotelFiltersDto> GetFiltersAsync(
        HotelSearchDto request,
        CancellationToken cancellationToken)
    {
        List<Hotel> hotels =
            await _hotelRepository.GetAllAsync(cancellationToken);

        List<decimal> prices = hotels
            .SelectMany(h => h.Rooms)
            .Select(r => r.PricePerNight)
            .ToList();

        decimal minPrice = prices.Any()
            ? prices.Min()
            : 0;

        decimal maxPrice = prices.Any()
            ? prices.Max()
            : 0;

        List<Hotel> ratingHotels = ApplyFilters(
            hotels,
            request,
            applyRating: false).ToList();

        List<Hotel> starsHotels = ApplyFilters(
            hotels,
            request,
            applyStars: false).ToList();

        List<Hotel> typeHotels = ApplyFilters(
            hotels,
            request,
            applyTypes: false).ToList();

        List<Hotel> chainHotels = ApplyFilters(
            hotels,
            request,
            applyChains: false).ToList();

        List<Hotel> amenityHotels = ApplyFilters(
            hotels,
            request,
            applyAmenities: false).ToList();

        List<RatingFilterDto> ratings = new()
        {
            new RatingFilterDto
            {
                MinRating = 9,
                Count = ratingHotels.Count(h =>
                    h.Reviews.Any() &&
                    h.Reviews.Average(r => r.Rating) >= 9)
            },
            new RatingFilterDto
            {
                MinRating = 8,
                Count = ratingHotels.Count(h =>
                    h.Reviews.Any() &&
                    h.Reviews.Average(r => r.Rating) >= 8)
            },
            new RatingFilterDto
            {
                MinRating = 7,
                Count = ratingHotels.Count(h =>
                    h.Reviews.Any() &&
                    h.Reviews.Average(r => r.Rating) >= 7)
            },
            new RatingFilterDto
            {
                MinRating = 6,
                Count = ratingHotels.Count(h =>
                    h.Reviews.Any() &&
                    h.Reviews.Average(r => r.Rating) >= 6)
            }
        };

        List<StarsFilterDto> stars = starsHotels
            .GroupBy(h => h.Stars)
            .OrderByDescending(g => g.Key)
            .Select(g => new StarsFilterDto
            {
                Stars = g.Key,
                Count = g.Count()
            })
            .ToList();

        List<HotelTypeFilterDto> types = typeHotels
            .GroupBy(h => h.Type)
            .Select(g => new HotelTypeFilterDto
            {
                Type = g.Key.ToString(),
                Count = g.Count()
            })
            .ToList();

        List<ChainFilterDto> chains = chainHotels
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

        List<AmenityFilterDto> amenities = amenityHotels
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
        string cacheKey = $"hotel:{id}";

        HotelDto? cached =
            await _cacheService.GetAsync<HotelDto>(cacheKey);

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

        HotelDto result = Map(hotel);

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
            throw new Exception("Hotel name is required");
        }

        if (dto.Stars < 1 || dto.Stars > 5)
        {
            throw new Exception("Hotel stars must be from 1 to 5");
        }

        if (dto.Rooms.Count < 1 || dto.Rooms.Count > 4)
        {
            throw new Exception("A hotel must have from 1 to 4 rooms");
        }

        Hotel hotel = new Hotel
        {
            Id = Guid.NewGuid(),
            Address = dto.Address,
            Name = dto.Name,
            City = dto.City,
            Country = dto.Country,
            Description = dto.Description,
            Stars = dto.Stars,
            Type = dto.Type,
            HotelType = string.IsNullOrWhiteSpace(dto.HotelType)
                ? dto.Type.ToString()
                : dto.HotelType,
            Attractions = dto.Attractions,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            IsPopular = dto.IsPopular,
            IsCityCentre = dto.IsCityCentre,
            IsPopularPlace = dto.IsPopularPlace,
            NearMetro = dto.NearMetro,
            NearAirport = dto.NearAirport,
            NearStation = dto.NearStation
        };

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
                    AmenityName = amenity.Trim()
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
            Room room = _mapper.Map<Room>(roomDto);

            room.Id = Guid.NewGuid();
            room.HotelId = hotel.Id;

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

    private HotelSearchResultDto BuildSearchResult(
        IEnumerable<Hotel> hotels,
        HotelSearchDto request)
    {
        IEnumerable<Hotel> result =
            ApplyFilters(hotels, request);

        string sort = request.SortBy ??
                      request.Sort ??
                      string.Empty;

        result = sort.ToLower() switch
        {
            "rating" or "best-rating" =>
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

        int total = result.Count();

        int page = Math.Max(request.Page, 1);

        int pageSize = request.PageSize <= 0
            ? 20
            : request.PageSize;

        List<HotelDto> hotelsResult = result
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(Map)
            .ToList();

        return new HotelSearchResultDto
        {
            Hotels = hotelsResult,
            Total = total,
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        };
    }

    private IEnumerable<Hotel> ApplyBaseFilters(
        IEnumerable<Hotel> hotels,
        HotelSearchDto request)
    {
        IEnumerable<Hotel> result = hotels;

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            string search = request.Search.Trim();

            result = result.Where(h =>
                h.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                h.City.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                h.Country.Contains(search, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(request.AttractionsSearch))
        {
            string attractionsSearch = request.AttractionsSearch.Trim();

            result = result.Where(h =>
                h.Attractions.Contains(
                    attractionsSearch,
                    StringComparison.OrdinalIgnoreCase));
        }

        int people = request.Adults + request.Children;

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
                    (!request.CheckIn.HasValue ||
                     !request.CheckOut.HasValue ||
                     !r.Bookings.Any(b =>
                         b.Status != "Cancelled" &&
                         b.CheckInDate < request.CheckOut &&
                         b.CheckOutDate > request.CheckIn))) >=
                request.Rooms);
        }

        return result;
    }

    private HotelDto Map(Hotel hotel)
    {
        bool hasReviews = hotel.Reviews.Count > 0;

        double rating = hasReviews
            ? Math.Round(
                hotel.Reviews.Average(x => x.Rating),
                1)
            : 0;

        double facilities = hasReviews
            ? Math.Round(
                hotel.Reviews.Average(x => x.Facilities),
                1)
            : 0;

        double staff = hasReviews
            ? Math.Round(
                hotel.Reviews.Average(x => x.Staff),
                1)
            : 0;

        double cleanliness = hasReviews
            ? Math.Round(
                hotel.Reviews.Average(x => x.Cleanliness),
                1)
            : 0;

        double comfort = hasReviews
            ? Math.Round(
                hotel.Reviews.Average(x => x.Comfort),
                1)
            : 0;

        double location = hasReviews
            ? Math.Round(
                hotel.Reviews.Average(x => x.Location),
                1)
            : 0;

        double valueForMoney = hasReviews
            ? Math.Round(
                hotel.Reviews.Average(x => x.ValueForMoney),
                1)
            : 0;

        List<string> images =
            hotel.Images
                .Select(x => x.ImageUrl)
                .ToList();

        List<ReviewDto> reviews =
            hotel.Reviews
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new ReviewDto
                {
                    Id = x.Id,
                    HotelId = x.HotelId,
                    AuthorName = x.User?.Name ?? "User",
                    AuthorAvatarUrl = x.User?.AvatarUrl,
                    Text = x.Comment,
                    Rating = x.Rating,
                    CreatedAt = x.CreatedAt,
                    Facilities = x.Facilities,
                    Staff = x.Staff,
                    Cleanliness = x.Cleanliness,
                    Comfort = x.Comfort,
                    Location = x.Location,
                    ValueForMoney = x.ValueForMoney
                })
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

            Type = hotel.Type,
            Stars = hotel.Stars,

            HotelType = string.IsNullOrWhiteSpace(hotel.HotelType)
                ? hotel.Type.ToString()
                : hotel.HotelType,

            HotelChain = hotel.HotelChain?.Name ?? string.Empty,
            Attractions = hotel.Attractions,

            Latitude = hotel.Latitude,
            Longitude = hotel.Longitude,
            MapUrl = mapUrl,

            IsPopular = hotel.IsPopular,
            IsCityCentre = hotel.IsCityCentre,
            IsPopularPlace = hotel.IsPopularPlace,
            NearMetro = hotel.NearMetro,
            NearAirport = hotel.NearAirport,
            NearStation = hotel.NearStation,

            Rating = rating,
            Facilities = facilities,
            Staff = staff,
            Cleanliness = cleanliness,
            Comfort = comfort,
            Location = location,
            ValueForMoney = valueForMoney,

            ReviewsCount = reviews.Count,
            MainImageUrl = images.FirstOrDefault() ?? string.Empty,
            Images = images,

            Amenities = hotel.Amenities
                .Select(x => x.AmenityName)
                .ToList(),

            HasWifi = hasWifi,

            Rooms = hotel.Rooms
                .Select(x => new RoomDto
                {
                    Id = x.Id,
                    HotelId = x.HotelId,
                    Title = x.Title,
                    BedType = x.BedType,
                    Capacity = x.Capacity,
                    PricePerNight = x.PricePerNight,
                    IsAvailable = x.IsAvailable,
                    ImageUrl = x.ImageUrl
                })
                .ToList(),

            Reviews = reviews
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
        IEnumerable<Hotel> result =
            ApplyBaseFilters(hotels, request);

        if (applyRating && request.MinRating.HasValue)
        {
            result = result.Where(h =>
                h.Reviews.Any() &&
                h.Reviews.Average(r => r.Rating) >=
                request.MinRating.Value);
        }

        if (applyStars && request.Stars.HasValue)
        {
            result = result.Where(h =>
                h.Stars == request.Stars.Value);
        }

        if (request.Popular == true)
        {
            result = result.Where(h => h.IsPopular);
        }

        if (request.CityCentre == true)
        {
            result = result.Where(h => h.IsCityCentre);
        }

        if (request.PopularPlaces == true)
        {
            result = result.Where(h => h.IsPopularPlace);
        }

        if (request.BestRating == true)
        {
            result = result.Where(h =>
                h.Reviews.Any() &&
                h.Reviews.Average(r => r.Rating) >= 9);
        }

        if (request.NearMetro == true)
        {
            result = result.Where(h => h.NearMetro);
        }

        if (request.NearAirport == true)
        {
            result = result.Where(h => h.NearAirport);
        }

        if (request.NearStation == true)
        {
            result = result.Where(h => h.NearStation);
        }

        if (!string.IsNullOrWhiteSpace(request.Near))
        {
            string near = request.Near.Trim();

            result = result.Where(h =>
                (h.NearMetro &&
                 near.Equals(
                     "metro",
                     StringComparison.OrdinalIgnoreCase)) ||
                (h.NearAirport &&
                 near.Equals(
                     "airport",
                     StringComparison.OrdinalIgnoreCase)) ||
                (h.NearStation &&
                 near.Equals(
                     "station",
                     StringComparison.OrdinalIgnoreCase)));
        }

        if (applyTypes &&
            request.Types != null &&
            request.Types.Length > 0)
        {
            result = result.Where(h =>
                request.Types.Contains(h.Type));
        }

        if (request.HotelTypes.Count > 0)
        {
            result = result.Where(h =>
                request.HotelTypes.Any(type =>
                    h.HotelType.Equals(
                        type,
                        StringComparison.OrdinalIgnoreCase) ||
                    h.Type.ToString().Equals(
                        type,
                        StringComparison.OrdinalIgnoreCase)));
        }

        if (applyChains &&
            request.ChainIds != null &&
            request.ChainIds.Length > 0)
        {
            result = result.Where(h =>
                h.HotelChainId.HasValue &&
                request.ChainIds.Contains(h.HotelChainId.Value));
        }

        if (request.ChainHotels.Count > 0)
        {
            result = result.Where(h =>
                h.HotelChain != null &&
                request.ChainHotels.Any(chain =>
                    h.HotelChain.Name.Equals(
                        chain,
                        StringComparison.OrdinalIgnoreCase)));
        }

        if (applyAmenities &&
            request.Amenities != null &&
            request.Amenities.Length > 0)
        {
            foreach (string amenity in request.Amenities)
            {
                result = result.Where(h =>
                    h.Amenities.Any(a =>
                        a.AmenityName.Equals(
                            amenity,
                            StringComparison.OrdinalIgnoreCase)));
            }
        }

        if (request.Facilities.Count > 0)
        {
            foreach (string facility in request.Facilities)
            {
                result = result.Where(h =>
                    h.Amenities.Any(a =>
                        a.AmenityName.Equals(
                            facility,
                            StringComparison.OrdinalIgnoreCase)));
            }
        }

        if (request.MinPrice.HasValue ||
            request.MaxPrice.HasValue)
        {
            result = result.Where(h =>
                h.Rooms.Any(r =>
                    (!request.MinPrice.HasValue ||
                     r.PricePerNight >= request.MinPrice.Value) &&
                    (!request.MaxPrice.HasValue ||
                     r.PricePerNight <= request.MaxPrice.Value)));
        }

        return result;
    }

    public async Task<List<HotelDto>?> GetRandomAsync(
        int number,
        CancellationToken cancellationToken)
    {
        string cacheKey = $"hotels:random:{number}";

        List<HotelDto>? cached =
            await _cacheService.GetAsync<List<HotelDto>>(cacheKey);

        if (cached != null)
        {
            return cached;
        }

        List<Hotel>? hotels =
            await _hotelRepository.GetRandomHotelsAsync(
                number,
                cancellationToken);

        List<HotelDto> result =
            hotels?
                .Select(Map)
                .ToList() ??
            new List<HotelDto>();

        await _cacheService.SetAsync(
            cacheKey,
            result,
            null);

        return result;
    }
}