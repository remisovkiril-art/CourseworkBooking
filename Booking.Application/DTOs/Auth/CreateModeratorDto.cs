namespace Booking.Application.DTOs.Auth;

public class CreateModeratorDto
{
    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string? Name { get; set; }
}