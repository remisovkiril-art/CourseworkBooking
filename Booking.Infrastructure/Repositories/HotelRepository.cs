using Booking.Application.DTOs.Hotels;
using Booking.Application.Interfaces.Repository;
using Booking.Domain.Entities;
using Booking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Booking.Infrastructure.Repositories;

public class HotelRepository : IHotelRepository
{
    private readonly ApplicationDbContext _context;

    public HotelRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Hotel>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        return await _context.Hotels
            .AsNoTracking()
            .AsSplitQuery()
            .Include(x => x.HotelChain)
            .Include(x => x.Rooms)
                .ThenInclude(x => x.Bookings)
            .Include(x => x.Amenities)
            .Include(x => x.Images)
            .Include(x => x.Reviews)
                .ThenInclude(x => x.User)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Hotel>> SearchAsync(
        HotelSearchDto dto,
        CancellationToken cancellationToken)
    {
        IQueryable<Hotel> query = _context.Hotels
            .AsNoTracking()
            .AsSplitQuery()
            .Include(x => x.HotelChain)
            .Include(x => x.Rooms)
                .ThenInclude(x => x.Bookings)
            .Include(x => x.Amenities)
            .Include(x => x.Images)
            .Include(x => x.Reviews)
                .ThenInclude(x => x.User);

        if (!string.IsNullOrWhiteSpace(dto.Search))
        {
            string search = dto.Search.Trim();
            string pattern = search + "%";

            query = query.Where(x =>
                x.Name != null &&
                EF.Functions.Like(x.Name, pattern));
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<Hotel?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await _context.Hotels
            .AsNoTracking()
            .AsSplitQuery()
            .Include(x => x.HotelChain)
            .Include(x => x.Rooms)
                .ThenInclude(x => x.Bookings)
            .Include(x => x.Amenities)
            .Include(x => x.Images)
            .Include(x => x.Reviews)
                .ThenInclude(x => x.User)
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task AddAsync(
        Hotel hotel,
        CancellationToken cancellationToken)
    {
        await _context.Hotels.AddAsync(
            hotel,
            cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(
        Hotel hotel,
        CancellationToken cancellationToken)
    {
        _context.Hotels.Update(hotel);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task AddImageAsync(
        HotelImage image,
        CancellationToken cancellationToken)
    {
        await _context.HotelImages.AddAsync(
            image,
            cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<Hotel>?> GetRandomHotelsAsync(
        int number,
        CancellationToken cancellationToken)
    {
        if (number <= 0)
        {
            return new List<Hotel>();
        }

        return await _context.Hotels
            .AsNoTracking()
            .AsSplitQuery()
            .OrderBy(x => Guid.NewGuid())
            .Take(number)
            .Include(x => x.HotelChain)
            .Include(x => x.Rooms)
                .ThenInclude(x => x.Bookings)
            .Include(x => x.Amenities)
            .Include(x => x.Images)
            .Include(x => x.Reviews)
                .ThenInclude(x => x.User)
            .ToListAsync(cancellationToken);
    }

    public async Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        Hotel? hotel = await _context.Hotels
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);

        if (hotel == null)
        {
            throw new Exception("Hotel not found");
        }

        _context.Hotels.Remove(hotel);

        await _context.SaveChangesAsync(cancellationToken);
    }
}