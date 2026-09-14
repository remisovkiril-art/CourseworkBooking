using Booking.Domain.Entities;

namespace Booking.Application.Interfaces.Repository;

public interface IRoomRepository
{
    Task<Room?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<List<Room>> GetByHotelIdAsync(
        Guid hotelId,
        CancellationToken cancellationToken);

    Task<int> CountByHotelIdAsync(
        Guid hotelId,
        CancellationToken cancellationToken);

    Task AddAsync(
        Room room,
        CancellationToken cancellationToken);
}
