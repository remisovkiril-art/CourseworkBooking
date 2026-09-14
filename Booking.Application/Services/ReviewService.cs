using Booking.Application.DTOs.Reviews;
using Booking.Application.Interfaces.Repository;
using Booking.Application.Interfaces.Services;
using Booking.Domain.Entities;

namespace Booking.Application.Services;

public class ReviewService : IReviewService
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IHotelRepository _hotelRepository;

    public ReviewService(
        IReviewRepository reviewRepository,
        IHotelRepository hotelRepository)
    {
        _reviewRepository = reviewRepository;
        _hotelRepository = hotelRepository;
    }

    public async Task<List<ReviewDto>> GetByHotelIdAsync(
        Guid hotelId,
        CancellationToken cancellationToken)
    {
        var reviews = await _reviewRepository.GetByHotelIdAsync(
            hotelId,
            cancellationToken);

        return reviews.Select(x => new ReviewDto
        {
            Id = x.Id,
            HotelId = x.HotelId,
            AuthorName = x.User.Name,
            Text = x.Comment,
            Rating = x.Rating,
            CreatedAt = x.CreatedAt
        }).ToList();
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

        var review = new Review
        {
            Id = Guid.NewGuid(),
            HotelId = dto.HotelId,
            UserId = userId,
            Rating = dto.Rating,
            Comment = dto.Text,
            CreatedAt = DateTime.UtcNow
        };

        await _reviewRepository.AddAsync(
            review,
            cancellationToken);

        return new ReviewDto
        {
            Id = review.Id,
            HotelId = review.HotelId,
            AuthorName = "User",
            Text = review.Comment,
            Rating = review.Rating,
            CreatedAt = review.CreatedAt
        };
    }
}
