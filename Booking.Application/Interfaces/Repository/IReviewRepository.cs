using Booking.Domain.Entities;

namespace Booking.Application.Interfaces.Repository;

public interface IReviewRepository
{
    Task<List<Review>> GetByHotelIdAsync(
        Guid hotelId,
        CancellationToken cancellationToken);

    Task AddAsync(
        Review review,
        CancellationToken cancellationToken);
}
