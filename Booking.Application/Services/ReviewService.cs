using AutoMapper;
using Booking.Application.DTOs.Hotels;
using Booking.Application.DTOs.Reviews;
using Booking.Application.Interfaces.Repository;
using Booking.Application.Interfaces.Services;
using Booking.Domain.Entities;

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
        var cache = await _cachingService.GetAsync<List<ReviewDto>>(cacheKey);
        if (cache == null)
        {
            var reviews = await _reviewRepository.GetByHotelIdAsync(
            hotelId,
            cancellationToken);
            cache = _mapper.Map<List<ReviewDto>>(reviews);
            await _cachingService.SetAsync(cacheKey, cache, null);

        }
        return cache;
        //var reviews = await _reviewRepository.GetByHotelIdAsync(
        //    hotelId,
        //    cancellationToken);

        //return reviews.Select(x => _mapper.Map<ReviewDto>(x)).ToList();
    }

    public async Task<ReviewDto> AddAsync(
        Guid userId,
        CreateReviewDto dto,
        CancellationToken cancellationToken)
    {
        if (dto.Rating < 1 || dto.Rating > 5)
        {
            throw new Exception(
                "Rating must be from 1 to 5");
        }

        if (string.IsNullOrWhiteSpace(dto.Text))
        {
            throw new Exception(
                "Review text is required");
        }

        var hotel = await _hotelRepository.GetByIdAsync(
            dto.HotelId,
            cancellationToken);

        if (hotel == null)
        {
            throw new Exception("Hotel not found");
        }

        var review = _mapper.Map<Review>(dto);
        review.Id = Guid.NewGuid();
        review.UserId = userId;
        review.CreatedAt = DateTime.UtcNow;

        await _reviewRepository.AddAsync(
            review,
            cancellationToken);

        await _cachingService.RemoveAsync($"reviews:hotel:{dto.HotelId}");

        return _mapper.Map<ReviewDto>(review);

    }
}
