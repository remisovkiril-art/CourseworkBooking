namespace Booking.Application.DTOs.Auth;

public class UpdateUserDto
{
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string TravelPurpose { get; set; } = string.Empty;
    public bool TravelingWithPet { get; set; }
}