namespace Booking.Application.DTOs.Auth;

public class PaymentMethodDto
{
    public Guid Id { get; set; }

    public string CardType { get; set; } = string.Empty;

    public string CardNumberHidden { get; set; } = string.Empty;

    public string Last4 { get; set; } = string.Empty;

    public string ExpirationDate { get; set; } = string.Empty;
}