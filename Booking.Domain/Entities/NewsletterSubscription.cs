using System;

namespace Booking.Domain.Entities;

public class NewsletterSubscription
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string Email { get; set; } = string.Empty;

    public bool SeasonalOffers { get; set; }

    public bool FavoriteCities { get; set; }

    public bool AcrossTheWorld { get; set; }

    public bool AffordableTravel { get; set; }

    public User User { get; set; } = null!;
}