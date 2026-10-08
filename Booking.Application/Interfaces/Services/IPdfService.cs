namespace Booking.Application.Interfaces.Services;

public interface IPdfService
{
    Task<byte[]> GenerateBookingPdfAsync(
        Guid bookingId,
        Guid userId,
        CancellationToken cancellationToken);
}