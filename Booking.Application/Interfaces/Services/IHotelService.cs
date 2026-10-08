using Booking.Application.DTOs.Hotels;

namespace Booking.Application.Interfaces.Services;

public interface IHotelService
{
    Task<HotelSearchResultDto> GetAllAsync(
        HotelSearchDto request,
        CancellationToken cancellationToken);

    Task<HotelSearchResultDto> SearchAsync(
        HotelSearchDto dto,
        CancellationToken cancellationToken);

    Task<HotelFiltersDto> GetFiltersAsync(
        HotelSearchDto request,
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

    Task<List<HotelDto>?> GetRandomAsync(
        int number,
        CancellationToken cancellationToken);

    Task<List<HotelFavoriteDto>?> GetFavoritesAsync(
    List<Guid> ids,
    CancellationToken cancellationToken);
}
