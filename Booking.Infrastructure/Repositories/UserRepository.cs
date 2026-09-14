using Booking.Application.Interfaces.Repository;
using Booking.Domain.Entities;
using Booking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Booking.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken)
    {
        return await _context.Users
            .FirstOrDefaultAsync(
                x => x.Email == email,
                cancellationToken);
    }

    public async Task<User?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await _context.Users
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<User?> GetByRefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken)
    {
        return await _context.Users
            .FirstOrDefaultAsync(
                x => x.RefreshToken == refreshToken,
                cancellationToken);
    }

    public async Task AddAsync(
        User user,
        CancellationToken cancellationToken)
    {
        await _context.Users.AddAsync(
            user,
            cancellationToken);

        await _context.SaveChangesAsync(
            cancellationToken);
    }

    public async Task UpdateAsync(
        User user,
        CancellationToken cancellationToken)
    {
        _context.Users.Update(user);

        await _context.SaveChangesAsync(
            cancellationToken);
    }
}
