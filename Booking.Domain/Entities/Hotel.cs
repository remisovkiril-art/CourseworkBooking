using Booking.Domain.Enum;

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

    public string HotelType { get; set; } = string.Empty;
    public string Attractions { get; set; } = string.Empty;

    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public bool IsPopular { get; set; }
    public bool IsCityCentre { get; set; }
    public bool IsPopularPlace { get; set; }
    public bool NearMetro { get; set; }
    public bool NearAirport { get; set; }
    public bool NearStation { get; set; }

    public ICollection<Room> Rooms { get; set; } = new List<Room>();
    public ICollection<HotelAmenity> Amenities { get; set; } = new List<HotelAmenity>();
    public ICollection<HotelImage> Images { get; set; } = new List<HotelImage>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}
