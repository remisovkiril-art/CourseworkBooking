using Booking.Application.Interfaces.Repository;
using Booking.Domain.Entities;
using Booking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Booking.Infrastructure.Repositories;

public class ReviewRepository : IReviewRepository
{
    private readonly ApplicationDbContext _context;

    public ReviewRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Review>> GetByHotelIdAsync(
        Guid hotelId,
        CancellationToken cancellationToken)
    {
        return await _context.Reviews
            .Include(x => x.User)
            .Where(x => x.HotelId == hotelId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(
        Review review,
        CancellationToken cancellationToken)
    {
        await _context.Reviews.AddAsync(
            review,
            cancellationToken);

        await _context.SaveChangesAsync(
            cancellationToken);
    }
}


