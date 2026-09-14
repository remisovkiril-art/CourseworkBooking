using Booking.Application.DTOs.Auth;
using Booking.Application.Interfaces.Repository;
using Booking.Application.Interfaces.Services;
using Booking.Domain.Entities;

namespace Booking.Application.Services;

public class PaymentMethodService : IPaymentMethodService
{
    private readonly IPaymentMethodRepository _repository;

    public PaymentMethodService(IPaymentMethodRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<PaymentMethod>> GetAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        return await _repository.GetByUserIdAsync(
            userId,
            cancellationToken);
    }

    public async Task AddAsync(
        Guid userId,
        AddPaymentMethodDto dto,
        CancellationToken cancellationToken)
    {
        var cardNumber = dto.CardNumber.Replace(" ", "");

        if (cardNumber.Length != 16 || !cardNumber.All(char.IsDigit))
            throw new Exception("Invalid card number");

        var last4 = cardNumber[^4..];

        var paymentMethod = new PaymentMethod
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CardType = dto.CardType,
            CardNumberHidden = $"**** **** **** {last4}",
            Last4 = last4,
            ExpirationDate = dto.ExpirationDate
        };

        await _repository.AddAsync(
            paymentMethod,
            cancellationToken);
    }
}
