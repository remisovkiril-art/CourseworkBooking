using Booking.Application.Interfaces.Repository;
using Booking.Domain.Entities;
using Booking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Booking.Infrastructure.Repositories;

public class PaymentMethodRepository : IPaymentMethodRepository
{
    private readonly ApplicationDbContext _context;

    public PaymentMethodRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<PaymentMethod>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        return await _context.PaymentMethods
            .Where(x => x.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(
        PaymentMethod paymentMethod,
        CancellationToken cancellationToken)
    {
        await _context.PaymentMethods.AddAsync(
            paymentMethod,
            cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
