using Booking.Domain.Entities;

namespace Booking.Application.Interfaces.Repository;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);

    Task<User?> GetByIdAsync(Guid id);

    Task AddAsync(User user);

    Task UpdateAsync(User user);
}