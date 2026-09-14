using Booking.Application.DTOs.Auth;
using Booking.Domain.Entities;

namespace Booking.Application.Interfaces.Services;

public interface IPaymentMethodService
{
    Task<List<PaymentMethod>> GetAsync(
        Guid userId,
        CancellationToken cancellationToken);

    Task AddAsync(
        Guid userId,
        AddPaymentMethodDto dto,
        CancellationToken cancellationToken);
}
