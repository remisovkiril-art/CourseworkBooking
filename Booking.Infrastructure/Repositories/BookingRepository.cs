using Booking.Application.Interfaces.Repository;
using Booking.Domain.Entities;
using Booking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Booking.Infrastructure.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly ApplicationDbContext _context;

    public BookingRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<BookingEntity>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        return await _context.Bookings
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(
        BookingEntity booking,
        CancellationToken cancellationToken)
    {
        await _context.Bookings.AddAsync(
            booking,
            cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> RoomIsBookedAsync(
        Guid roomId,
        DateTime checkIn,
        DateTime checkOut,
        CancellationToken cancellationToken)
    {
        return await _context.Bookings.AnyAsync(
            x =>
                x.RoomId == roomId &&
                x.Status != "Cancelled" &&
                x.CheckInDate < checkOut &&
                x.CheckOutDate > checkIn,
            cancellationToken);
    }
}