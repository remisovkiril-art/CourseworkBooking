namespace Booking.Application.DTOs.Auth;

public class AddPaymentMethodDto
{
    public string CardType { get; set; } = string.Empty;
    public string CardNumber { get; set; } = string.Empty;
    public string ExpirationDate { get; set; } = string.Empty;
}