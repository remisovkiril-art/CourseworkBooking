using System;

namespace Booking.Domain.Entities;

public class HotelAmenity
{
    public Guid Id { get; set; }
    public Guid HotelId { get; set; }
    public string AmenityName { get; set; } = string.Empty;
    public Hotel Hotel { get; set; } = null!;
}