using Booking.Application.DTOs.Hotels;

namespace Booking.Application.Interfaces.Services;

public interface IHotelService
{
    Task<HotelSearchResultDto> SearchAsync(
        HotelSearchDto dto,
        CancellationToken cancellationToken);

    Task<HotelDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<HotelDto> CreateAsync(
        HotelCreateDto dto,
        CancellationToken cancellationToken);

    Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken);
}