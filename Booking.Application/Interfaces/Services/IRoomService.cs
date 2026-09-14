using Booking.Application.DTOs.Hotels;

namespace Booking.Application.Interfaces.Services;

public interface IRoomService
{
    Task<List<RoomDto>> GetByHotelIdAsync(
        Guid hotelId,
        CancellationToken cancellationToken);

    Task<RoomDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<RoomDto> CreateAsync(
        Guid hotelId,
        RoomCreateDto dto,
        CancellationToken cancellationToken);
}
