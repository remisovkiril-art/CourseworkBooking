using System;

namespace Booking.Domain.Entities;

public class HotelImage
{
    public Guid Id { get; set; }

    public Guid HotelId { get; set; }

    public string ImageUrl { get; set; } = string.Empty;

    public Hotel Hotel { get; set; } = null!;
}

