using System;

namespace Booking.Domain.Entities;

public class UserTravelPreference
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string Category { get; set; } = string.Empty;

    public string PreferenceName { get; set; } = string.Empty;

    public User User { get; set; } = null!;
}