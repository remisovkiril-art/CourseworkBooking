using Booking.Application.DTOs.Reviews;

namespace Booking.Application.Interfaces.Services;

public interface IReviewService
{
    Task<List<ReviewDto>> GetByHotelIdAsync(
        Guid hotelId,
        CancellationToken cancellationToken);

    Task<ReviewDto> AddAsync(
        Guid userId,
        CreateReviewDto dto,
        CancellationToken cancellationToken);

    Task<List<ReviewDto>?> GetBestReviewsAsync(
        int count,
        CancellationToken cancellationToken);

    Task DeleteAsync(
        Guid reviewId,
        CancellationToken cancellationToken);
}
