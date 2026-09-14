using Booking.Application.Interfaces.Repository;
using Booking.Domain.Entities;
using Booking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Booking.Infrastructure.Repositories;

public class RoomRepository : IRoomRepository
{
    private readonly ApplicationDbContext _context;

    public RoomRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Room?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await _context.Rooms
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<List<Room>> GetByHotelIdAsync(
        Guid hotelId,
        CancellationToken cancellationToken)
    {
        return await _context.Rooms
            .Where(x => x.HotelId == hotelId)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountByHotelIdAsync(
        Guid hotelId,
        CancellationToken cancellationToken)
    {
        return await _context.Rooms
            .CountAsync(x => x.HotelId == hotelId, cancellationToken);
    }

    public async Task AddAsync(
        Room room,
        CancellationToken cancellationToken)
    {
        await _context.Rooms.AddAsync(room, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
