using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Booking.Domain.Entities;

public class Room
{
    public Guid Id { get; set; }
    public Guid HotelId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string BedType { get; set; } = string.Empty;
    public int Capacity { get; set; } = 2;

    [Column(TypeName = "decimal(18,2)")]
    public decimal PricePerNight { get; set; }

    public bool IsAvailable { get; set; } = true;
    public string ImageUrl { get; set; } = string.Empty;

    public Hotel Hotel { get; set; } = null!;
    public ICollection<BookingEntity> Bookings { get; set; } = new List<BookingEntity>();
}