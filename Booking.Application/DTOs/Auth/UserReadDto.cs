namespace Booking.Application.DTOs.Auth;

public class UserReadDto
{
    public Guid Id { get; set; }

    public string Email { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;
}