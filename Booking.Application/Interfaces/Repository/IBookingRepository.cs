using Booking.Domain.Entities;

namespace Booking.Application.Interfaces.Repository;

public interface IBookingRepository
{
    Task<List<BookingEntity>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken);

    Task AddAsync(
        BookingEntity booking,
        CancellationToken cancellationToken);

    Task<bool> RoomIsBookedAsync(
        Guid roomId,
        DateTime checkIn,
        DateTime checkOut,
        CancellationToken cancellationToken);
}