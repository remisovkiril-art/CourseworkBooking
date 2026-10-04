using AutoMapper;
using Booking.Application.DTOs.Hotels;
using Booking.Application.DTOs.Reviews;
using Booking.Application.Interfaces.Repository;
using Booking.Application.Interfaces.Services;
using Booking.Domain.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Booking.Application.Services;

public class ReviewService : IReviewService
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IHotelRepository _hotelRepository;
    private readonly IMapper _mapper;
    private readonly ICachingService _cachingService;

    public ReviewService(
        IReviewRepository reviewRepository,
        IHotelRepository hotelRepository,
        IMapper mapper,
        ICachingService cachingService)
    {
        _reviewRepository = reviewRepository;
        _hotelRepository = hotelRepository;
        _mapper = mapper;
        _cachingService = cachingService;
    }

    public async Task<List<ReviewDto>> GetByHotelIdAsync(
        Guid hotelId,
        CancellationToken cancellationToken)
    {
        var cacheKey = $"reviews:hotel:{hotelId}";

        var cache = await _cachingService
            .GetAsync<List<ReviewDto>>(cacheKey);

        if (cache == null)
        {
            var reviews = await _reviewRepository.GetByHotelIdAsync(
                hotelId,
                cancellationToken);

            cache = _mapper.Map<List<ReviewDto>>(reviews);

            await _cachingService.SetAsync(
                cacheKey,
                cache,
                null);
        }

        return cache;
    }

    public async Task<ReviewDto> AddAsync(
        Guid userId,
        CreateReviewDto dto,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(dto.Text))
        {
            throw new Exception("Review text is required");
        }

        if (dto.Facilities < 1 || dto.Facilities > 10 ||
            dto.Staff < 1 || dto.Staff > 10 ||
            dto.Cleanliness < 1 || dto.Cleanliness > 10 ||
            dto.Comfort < 1 || dto.Comfort > 10 ||
            dto.Location < 1 || dto.Location > 10 ||
            dto.ValueForMoney < 1 || dto.ValueForMoney > 10)
        {
            throw new Exception(
                "All ratings must be from 1 to 10");
        }

        var hotel = await _hotelRepository.GetByIdAsync(
            dto.HotelId,
            cancellationToken);

        if (hotel == null)
        {
            throw new Exception("Hotel not found");
        }

        var rating = (
            dto.Facilities +
            dto.Staff +
            dto.Cleanliness +
            dto.Comfort +
            dto.Location +
            dto.ValueForMoney
        ) / 6;

        var review = new Review
        {
            Id = Guid.NewGuid(),
            HotelId = dto.HotelId,
            UserId = userId,

            Rating = Math.Round(
                rating,
                1),

            Facilities = dto.Facilities,
            Staff = dto.Staff,
            Cleanliness = dto.Cleanliness,
            Comfort = dto.Comfort,
            Location = dto.Location,
            ValueForMoney = dto.ValueForMoney,

            Comment = dto.Text,
            CreatedAt = DateTime.UtcNow
        };

        await _reviewRepository.AddAsync(
            review,
            cancellationToken);

        await _cachingService.RemoveAsync(
            $"reviews:hotel:{dto.HotelId}");

        return new ReviewDto
        {
            Id = review.Id,
            HotelId = review.HotelId,
            AuthorName = "User",
            Text = review.Comment,
            Rating = review.Rating,
            Facilities = review.Facilities,
            Staff = review.Staff,
            Cleanliness = review.Cleanliness,
            Comfort = review.Comfort,
            Location = review.Location,
            ValueForMoney = review.ValueForMoney,
            CreatedAt = review.CreatedAt
        };
    }

    public async Task<List<ReviewDto>?> GetBestReviewsAsync(int count, CancellationToken cancellationToken)
    {
        var cacheKey = $"reviews:best:{count}";

        var cached = await _cachingService.GetAsync<List<ReviewDto>>(
            cacheKey);

        if (cached != null)
        {
            return cached;
        }

        var reviews = await _reviewRepository.GetBestReviewsAsync(
            count,
            cancellationToken);

        var result = _mapper.Map<List<ReviewDto>>(reviews);

        await _cachingService.SetAsync(
            cacheKey,
            result,
            null);

        return result;
    }
}