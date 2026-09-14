namespace Booking.Application.DTOs.Auth;

public class AuthResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    public DateTime AccessTokenExpires { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public string VerificationCode { get; set; } = string.Empty;
}
