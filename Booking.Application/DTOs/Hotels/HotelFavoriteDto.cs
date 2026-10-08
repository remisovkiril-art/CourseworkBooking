using Booking.Application.DTOs.Rooms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Application.DTOs.Hotels;

public class HotelFavoriteDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;

    public int Stars { get; set; }
    public double Rating { get; set; }
    public int ReviewsCount { get; set; }

    public string MainImageUrl { get; set; } = string.Empty;
    public List<string> Images { get; set; } = new();
    public List<RoomDto> Rooms { get; set; } = new();
}