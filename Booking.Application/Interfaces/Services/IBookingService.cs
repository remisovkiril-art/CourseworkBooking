using Booking.Application.DTOs.Bookings;

namespace Booking.Application.Interfaces.Services;

public interface IBookingService
{
    Task<BookingDto> CreateAsync(
        Guid userId,
        CreateBookingDto dto,
        CancellationToken cancellationToken);

    Task<List<BookingDto>> GetMyBookingsAsync(
        Guid userId,
        CancellationToken cancellationToken);
}