using System;

namespace Booking.Domain.Entities;
public class PaymentMethod
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string CardType { get; set; } = string.Empty;
    public string CardNumberHidden { get; set; } = string.Empty;
    public string ExpirationDate { get; set; } = string.Empty;
    public string Last4 { get; set; } = string.Empty;
    public User User { get; set; } = null!;
}
