using Booking.Domain.Entities;

namespace Booking.Application.Interfaces.Repository;

public interface IPaymentMethodRepository
{
    Task<List<PaymentMethod>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken);

    Task AddAsync(
        PaymentMethod paymentMethod,
        CancellationToken cancellationToken);
}
