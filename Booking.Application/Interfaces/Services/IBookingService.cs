using Booking.Application.DTOs.Bookings;

namespace Booking.Application.Interfaces.Services;

public interface IBookingService
{
    Task<object> CreateAsync(
        Guid userId,
        CreateBookingDto dto,
        CancellationToken cancellationToken);

    Task<List<object>> GetMyBookingsAsync(
        Guid userId,
        CancellationToken cancellationToken);
}
