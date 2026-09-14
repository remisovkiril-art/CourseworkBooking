using System;

namespace Booking.Domain.Entities;

public class Review
{
    public Guid Id { get; set; }

    public Guid HotelId { get; set; }

    public Guid UserId { get; set; }

    public int Rating { get; set; }

    public string Comment { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public Hotel Hotel { get; set; } = null!;

    public User User { get; set; } = null!;
}
