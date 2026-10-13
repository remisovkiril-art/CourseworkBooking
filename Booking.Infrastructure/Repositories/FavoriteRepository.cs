using Booking.Application.Interfaces.Repository;
using Booking.Domain.Entities;
using Booking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Booking.Infrastructure.Repositories;

public class FavoriteRepository : IFavoriteRepository
{
    private readonly ApplicationDbContext _context;

    public FavoriteRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<List<Guid>> GetHotelIdsAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        return _context.UserFavoriteHotels
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .Select(x => x.HotelId)
            .ToListAsync(cancellationToken);
    }

    public Task<List<Hotel>> GetHotelsAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        return _context.UserFavoriteHotels
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .Select(x => x.Hotel)
            .AsSplitQuery()
            .Include(x => x.HotelChain)
            .Include(x => x.Rooms).ThenInclude(x => x.Bookings)
            .Include(x => x.Amenities)
            .Include(x => x.Images)
            .Include(x => x.Reviews).ThenInclude(x => x.User)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(
        Guid userId,
        Guid hotelId,
        CancellationToken cancellationToken)
    {
        bool exists = await _context.Hotels
            .AnyAsync(x => x.Id == hotelId, cancellationToken);

        if (!exists)
        {
            throw new KeyNotFoundException("Hotel not found.");
        }

        bool alreadyFavorite = await _context.UserFavoriteHotels
            .AnyAsync(x => x.UserId == userId && x.HotelId == hotelId, cancellationToken);

        if (alreadyFavorite)
        {
            return;
        }

        await _context.UserFavoriteHotels.AddAsync(
            new UserFavoriteHotel { UserId = userId, HotelId = hotelId },
            cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveAsync(
        Guid userId,
        Guid hotelId,
        CancellationToken cancellationToken)
    {
        UserFavoriteHotel? favorite = await _context.UserFavoriteHotels
            .FirstOrDefaultAsync(
                x => x.UserId == userId && x.HotelId == hotelId,
                cancellationToken);

        if (favorite == null)
        {
            return;
        }

        _context.UserFavoriteHotels.Remove(favorite);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
