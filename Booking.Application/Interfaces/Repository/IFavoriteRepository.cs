using Booking.Domain.Entities;

namespace Booking.Application.Interfaces.Repository;

public interface IFavoriteRepository
{
    Task<List<Guid>> GetHotelIdsAsync(Guid userId, CancellationToken cancellationToken);
    Task<List<Hotel>> GetHotelsAsync(Guid userId, CancellationToken cancellationToken);
    Task AddAsync(Guid userId, Guid hotelId, CancellationToken cancellationToken);
    Task RemoveAsync(Guid userId, Guid hotelId, CancellationToken cancellationToken);
}
