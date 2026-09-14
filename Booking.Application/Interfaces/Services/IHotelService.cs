using Booking.Application.DTOs.Hotels;

namespace Booking.Application.Interfaces.Services;

public interface IHotelService
{
    Task<List<HotelDto>> GetAllAsync(
        string? search,
        int adults,
        int children,
        int rooms,
        DateTime? checkIn,
        DateTime? checkOut,
        CancellationToken cancellationToken);

    Task<HotelDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<HotelDto> CreateAsync(
        HotelCreateDto dto,
        CancellationToken cancellationToken);
}
