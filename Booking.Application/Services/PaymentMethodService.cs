using Booking.Application.DTOs.Auth;
using Booking.Application.Interfaces.Repository;
using Booking.Application.Interfaces.Services;
using Booking.Domain.Entities;

namespace Booking.Application.Services;

public class PaymentMethodService : IPaymentMethodService
{
    private readonly IPaymentMethodRepository _repository;

    public PaymentMethodService(
        IPaymentMethodRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<PaymentMethodDto>> GetAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        List<PaymentMethod> paymentMethods =
            await _repository.GetByUserIdAsync(
                userId,
                cancellationToken);

        return paymentMethods
            .Select(x => new PaymentMethodDto
            {
                Id = x.Id,
                CardType = x.CardType,
                CardNumberHidden = x.CardNumberHidden,
                Last4 = x.Last4,
                ExpirationDate = x.ExpirationDate
            })
            .ToList();
    }

    public async Task AddAsync(
        Guid userId,
        AddPaymentMethodDto dto,
        CancellationToken cancellationToken)
    {
        string cardNumber =
            dto.CardNumber.Replace(" ", "");

        if (cardNumber.Length != 16 ||
            !cardNumber.All(char.IsDigit))
        {
            throw new Exception("Invalid card number");
        }

        string last4 = cardNumber[^4..];

        PaymentMethod paymentMethod = new PaymentMethod
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CardType = dto.CardType,
            CardNumberHidden =
                $"**** **** **** {last4}",
            Last4 = last4,
            ExpirationDate = dto.ExpirationDate
        };

        await _repository.AddAsync(
            paymentMethod,
            cancellationToken);
    }
}