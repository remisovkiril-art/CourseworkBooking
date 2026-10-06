namespace Booking.Application.DTOs.Auth;

public class VerificationRequiredDto
{
    public string Email { get; set; } =
        string.Empty;

    public string Purpose { get; set; } =
        string.Empty;

    public string Message { get; set; } =
        string.Empty;
}