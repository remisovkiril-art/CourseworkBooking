using Booking.Domain.Enum;

namespace Booking.Application.DTOs.Hotels;

public class HotelSearchDto
{
    public string? Search { get; set; }
    public string? AttractionsSearch { get; set; }

    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }

    public double? MinRating { get; set; }
    public int? Stars { get; set; }

    public bool? Popular { get; set; }
    public bool? CityCentre { get; set; }
    public bool? PopularPlaces { get; set; }
    public bool? BestRating { get; set; }
    public bool? NearMetro { get; set; }
    public bool? NearAirport { get; set; }
    public bool? NearStation { get; set; }

    public List<string> Facilities { get; set; } = new();
    public List<string> HotelTypes { get; set; } = new();
    public List<string> ChainHotels { get; set; } = new();

    public int Adults { get; set; }
    public int Children { get; set; }
    public int Rooms { get; set; }

    public DateTime? CheckIn { get; set; }
    public DateTime? CheckOut { get; set; }

    public HotelType[]? Types { get; set; }
    public int[]? ChainIds { get; set; }
    public string[]? Amenities { get; set; }

    public string? Near { get; set; }

    public string? Sort { get; set; }
    public string? SortBy { get; set; }

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
