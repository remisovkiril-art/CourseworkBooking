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
}
