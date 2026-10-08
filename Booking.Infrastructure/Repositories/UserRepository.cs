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

    public async Task<List<UserTravelPreference>> GetTravelPreferencesAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        return await _context.UserTravelPreferences
            .Where(x => x.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task ReplaceTravelPreferencesAsync(
        Guid userId,
        List<UserTravelPreference> preferences,
        CancellationToken cancellationToken)
    {
        List<UserTravelPreference> oldPreferences =
            await _context.UserTravelPreferences
                .Where(x => x.UserId == userId)
                .ToListAsync(cancellationToken);

        _context.UserTravelPreferences.RemoveRange(
            oldPreferences);

        await _context.UserTravelPreferences.AddRangeAsync(
            preferences,
            cancellationToken);

        await _context.SaveChangesAsync(
            cancellationToken);
    }

    public async Task<NewsletterSubscription?> GetNewsletterAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        return await _context.NewsletterSubscriptions
            .FirstOrDefaultAsync(
                x => x.UserId == userId,
                cancellationToken);
    }

    public async Task SaveNewsletterAsync(
        NewsletterSubscription newsletter,
        CancellationToken cancellationToken)
    {
        NewsletterSubscription? existing =
            await _context.NewsletterSubscriptions
                .FirstOrDefaultAsync(
                    x => x.UserId == newsletter.UserId,
                    cancellationToken);

        if (existing == null)
        {
            newsletter.Id = Guid.NewGuid();

            await _context.NewsletterSubscriptions.AddAsync(
                newsletter,
                cancellationToken);
        }
        else
        {
            existing.Email = newsletter.Email;
            existing.SeasonalOffers = newsletter.SeasonalOffers;
            existing.FavoriteCities = newsletter.FavoriteCities;
            existing.AcrossTheWorld = newsletter.AcrossTheWorld;
            existing.AffordableTravel = newsletter.AffordableTravel;
        }

        await _context.SaveChangesAsync(
            cancellationToken);
    }
}
