using Booking.Application.DTOs.Hotels;
using Booking.Domain.Entities;

namespace Booking.Application.Interfaces.Repository;

public interface IHotelRepository
{
    Task<List<Hotel>> GetAllAsync(
        CancellationToken cancellationToken);

    Task<List<Hotel>> SearchAsync(
        HotelSearchDto dto,
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

    Task<List<Hotel>?> GetRandomHotelsAsync(
        int number,
        CancellationToken cancellationToken);

    Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken);
}