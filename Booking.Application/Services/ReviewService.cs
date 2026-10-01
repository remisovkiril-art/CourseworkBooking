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
            AuthorName = x.User.Name ?? "User",
            AuthorAvatarUrl = x.User.AvatarUrl,

            Text = x.Comment,
            Rating = x.Rating,

            Facilities = x.Facilities,
            Staff = x.Staff,
            Cleanliness = x.Cleanliness,
            Comfort = x.Comfort,
            Location = x.Location,
            ValueForMoney = x.ValueForMoney,

            CreatedAt = x.CreatedAt
        }).ToList();
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

            Rating = Math.Round(rating, 1),

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
}
