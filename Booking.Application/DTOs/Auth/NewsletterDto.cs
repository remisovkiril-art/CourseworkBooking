namespace Booking.Application.DTOs.Auth;

public class NewsletterDto
{
    public string Email { get; set; } = string.Empty;

    public bool SeasonalOffers { get; set; }

    public bool FavoriteCities { get; set; }

    public bool AcrossTheWorld { get; set; }

    public bool AffordableTravel { get; set; }
}