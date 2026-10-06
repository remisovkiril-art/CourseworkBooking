using Booking.Application.DTOs.Reviews;

namespace Booking.Application.DTOs.Hotels;

public class HotelDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int Stars { get; set; }

    public string HotelType { get; set; } = string.Empty;

    public string HotelChain { get; set; } = string.Empty;

    public string Attractions { get; set; } = string.Empty;

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public string MapUrl { get; set; } = string.Empty;

    public bool IsPopular { get; set; }

    public bool IsCityCentre { get; set; }

    public bool IsPopularPlace { get; set; }

    public bool NearMetro { get; set; }

    public bool NearAirport { get; set; }

    public bool NearStation { get; set; }

    public double Facilities { get; set; }

    public double Staff { get; set; }

    public double Cleanliness { get; set; }

    public double Comfort { get; set; }

    public double Location { get; set; }

    public double ValueForMoney { get; set; }

    public double Rating { get; set; }

    public int ReviewsCount { get; set; }

    public string MainImageUrl { get; set; } = string.Empty;

    public List<string> Images { get; set; } =
        new();

    public List<RoomDto> Rooms { get; set; } =
        new();

    public List<string> Amenities { get; set; } =
        new();

    public bool? HasWifi { get; set; }

    public List<ReviewDto> Reviews { get; set; } =
        new();
}