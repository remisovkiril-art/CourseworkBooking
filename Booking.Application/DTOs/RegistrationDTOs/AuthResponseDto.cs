namespace Booking.Application.DTOs.RegistrationDTOs;

public class AuthResponseDto
{
    public string AccessToken { get; set; } = string.Empty;

    public DateTime AccessTokenExpires { get; set; }
}