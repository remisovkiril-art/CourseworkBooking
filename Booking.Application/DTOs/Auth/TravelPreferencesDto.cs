namespace Booking.Application.DTOs.Auth;

public class TravelPreferencesDto
{
    public List<string> General { get; set; } = new();

    public List<string> Accessibility { get; set; } = new();

    public List<string> Languages { get; set; } = new();

    public List<string> Parking { get; set; } = new();

    public List<string> ReceptionServices { get; set; } = new();

    public List<string> CleaningServices { get; set; } = new();

    public List<string> EntertainmentAndFamily { get; set; } = new();

    public List<string> SafetyAndSecurity { get; set; } = new();
}