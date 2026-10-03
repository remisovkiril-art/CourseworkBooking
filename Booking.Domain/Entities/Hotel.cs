using Booking.Domain.Enum;
using System;
using System.Collections.Generic;

namespace Booking.Domain.Entities;

public class Hotel
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public HotelType Type { get; set; }

    public int Stars { get; set; }

    public int? HotelChainId { get; set; }

    public HotelChain? HotelChain { get; set; }

    public ICollection<Room> Rooms { get; set; } = new List<Room>();
    public ICollection<HotelAmenity> Amenities { get; set; } = new List<HotelAmenity>();
    public ICollection<HotelImage> Images { get; set; } = new List<HotelImage>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}