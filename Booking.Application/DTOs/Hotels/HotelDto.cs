using Booking.Application.DTOs.Reviews;

namespace Booking.Application.DTOs.Hotels;

public class HotelDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double Rating { get; set; }
    public int ReviewsCount { get; set; }
    public string MainImageUrl { get; set; } = string.Empty;
    public List<string> Images { get; set; } = new();
    public List<RoomDto> Rooms { get; set; } = new();
    public List<string> Amenities { get; set; } = new();
    public bool? HasWifi { get; set; }
    public List<ReviewDto> Reviews { get; set; } = new();
}
