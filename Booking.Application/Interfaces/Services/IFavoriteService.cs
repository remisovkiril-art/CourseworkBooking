namespace Booking.Application.Interfaces.Services;

public interface IFavoriteService
{
    Task<List<Guid>> GetHotelIdsAsync(Guid userId, CancellationToken cancellationToken);
    Task AddAsync(Guid userId, Guid hotelId, CancellationToken cancellationToken);
    Task RemoveAsync(Guid userId, Guid hotelId, CancellationToken cancellationToken);
}
