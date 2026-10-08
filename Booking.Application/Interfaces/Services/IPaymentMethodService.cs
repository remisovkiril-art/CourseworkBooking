using Booking.Application.DTOs.Auth;

namespace Booking.Application.Interfaces.Services;

public interface IPaymentMethodService
{
    Task<List<PaymentMethodDto>> GetAsync(
        Guid userId,
        CancellationToken cancellationToken);

    Task AddAsync(
        Guid userId,
        AddPaymentMethodDto dto,
        CancellationToken cancellationToken);
}