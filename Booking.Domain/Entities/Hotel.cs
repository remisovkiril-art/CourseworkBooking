using System;
using System.Collections.Generic;

namespace Booking.Domain.Entities;

public class Hotel
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public double Rating { get; set; }

    public string MainImageUrl { get; set; } = string.Empty;

    public ICollection<Room> Rooms { get; set; } = new List<Room>();

    public ICollection<HotelAmenity> Amenities { get; set; }
        = new List<HotelAmenity>();
}