namespace Booking.Application.DTOs.Auth;

public class UpdateUserDto
{
    public string? Name { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? AvatarUrl { get; set; }

    public string? Country { get; set; }

    public string? City { get; set; }

    public string? PreferredCurrency { get; set; }

    public string? TravelPurpose { get; set; }

    public bool? TravelingWithPet { get; set; }

    public string? Gender { get; set; }

    public DateTime? DateOfBirth { get; set; }
}