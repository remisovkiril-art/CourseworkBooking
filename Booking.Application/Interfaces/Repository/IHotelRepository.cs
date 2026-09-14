using Booking.Domain.Entities;

namespace Booking.Application.Interfaces.Repository;

public interface IHotelRepository
{
    Task<List<Hotel>> GetAllAsync(
        CancellationToken cancellationToken);

    Task<Hotel?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task AddAsync(
        Hotel hotel,
        CancellationToken cancellationToken);

    Task UpdateAsync(
        Hotel hotel,
        CancellationToken cancellationToken);

    Task AddImageAsync(
        HotelImage image,
        CancellationToken cancellationToken);
}
