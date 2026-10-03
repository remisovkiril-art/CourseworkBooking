using System;
using System.ComponentModel.DataAnnotations;

namespace Booking.Domain.Entities;

public class Review
{
    public Guid Id { get; set; }

    public Guid HotelId { get; set; }

    public Guid UserId { get; set; }

    public double Rating { get; set; }

    [Range(1, 10)]
    public double Facilities { get; set; }

    [Range(1, 10)]
    public double Staff { get; set; }

    [Range(1, 10)]
    public double Cleanliness { get; set; }

    [Range(1, 10)]
    public double Comfort { get; set; }

    [Range(1, 10)]
    public double Location { get; set; }

    [Range(1, 10)]
    public double ValueForMoney { get; set; }
    public string Comment { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public Hotel Hotel { get; set; } = null!;

    public User User { get; set; } = null!;
}
