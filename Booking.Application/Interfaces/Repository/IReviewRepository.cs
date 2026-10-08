using Booking.Domain.Entities;

namespace Booking.Application.Interfaces.Repository;

public interface IReviewRepository
{
    Task<List<Review>> GetByHotelIdAsync(
        Guid hotelId,
        CancellationToken cancellationToken);

    Task<Review?> GetByIdAsync(
        Guid reviewId,
        CancellationToken cancellationToken);

    Task AddAsync(
        Review review,
        CancellationToken cancellationToken);

    Task<List<Review>?> GetBestReviewsAsync(
        int count,
        CancellationToken cancellationToken);

    Task DeleteAsync(
        Guid reviewId,
        CancellationToken cancellationToken);
}
