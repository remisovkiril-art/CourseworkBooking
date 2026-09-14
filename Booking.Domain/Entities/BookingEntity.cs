using System;

namespace Booking.Domain.Entities;

public class BookingEntity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid RoomId { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public int AdultsCount { get; set; }
    public int ChildrenCount { get; set; }
    public string TravelDetails { get; set; } = string.Empty;
    public decimal TotalPrice { get; set; }
    public bool IsPaid { get; set; }
    public string Status { get; set; } = "Pending";
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public User User { get; set; } = null!;
    public Room Room { get; set; } = null!;
}