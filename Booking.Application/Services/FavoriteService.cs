using Booking.Application.Interfaces.Repository;
using Booking.Application.Interfaces.Services;

namespace Booking.Application.Services;

public class FavoriteService : IFavoriteService
{
    private readonly IFavoriteRepository _favoriteRepository;

    public FavoriteService(IFavoriteRepository favoriteRepository)
    {
        _favoriteRepository = favoriteRepository;
    }

    public Task<List<Guid>> GetHotelIdsAsync(Guid userId, CancellationToken cancellationToken)
    {
        return _favoriteRepository.GetHotelIdsAsync(userId, cancellationToken);
    }

    public Task AddAsync(Guid userId, Guid hotelId, CancellationToken cancellationToken)
    {
        return _favoriteRepository.AddAsync(userId, hotelId, cancellationToken);
    }

    public Task RemoveAsync(Guid userId, Guid hotelId, CancellationToken cancellationToken)
    {
        return _favoriteRepository.RemoveAsync(userId, hotelId, cancellationToken);
    }
}
