namespace Booking.Application.DTOs.Hotels;

public class HotelCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool? HasWifi { get; set; }
    public List<string> Amenities { get; set; } = new();
    public List<RoomCreateDto> Rooms { get; set; } = new();
}
