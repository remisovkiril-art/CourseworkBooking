using Booking.Domain.Entities;

namespace Booking.Application.Interfaces.Repository;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken);

    Task<User?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<User?> GetByRefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken);

    Task AddAsync(
        User user,
        CancellationToken cancellationToken);

    Task UpdateAsync(
        User user,
        CancellationToken cancellationToken);
}
