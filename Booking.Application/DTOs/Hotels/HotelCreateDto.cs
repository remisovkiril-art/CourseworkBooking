using Booking.Application.DTOs.Rooms;
using Booking.Domain.Enum;

namespace Booking.Application.DTOs.Hotels;

public class HotelCreateDto
{
    public string Name { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public int Stars { get; set; }

    public HotelType Type { get; set; }

    public string HotelType { get; set; } = string.Empty;

    public string HotelChain { get; set; } = string.Empty;

    public string Attractions { get; set; } = string.Empty;

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public bool IsPopular { get; set; }

    public bool IsCityCentre { get; set; }

    public bool IsPopularPlace { get; set; }

    public bool NearMetro { get; set; }

    public bool NearAirport { get; set; }

    public bool NearStation { get; set; }

    public string City { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public bool? HasWifi { get; set; }

    public List<string> Amenities { get; set; } = new();

    public List<RoomCreateDto> Rooms { get; set; } = new();
}