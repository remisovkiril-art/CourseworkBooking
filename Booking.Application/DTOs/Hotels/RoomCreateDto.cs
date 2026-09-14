namespace Booking.Application.DTOs.Hotels;

public class RoomCreateDto
{
    public string Title { get; set; } = string.Empty;
    public string BedType { get; set; } = string.Empty;
    public int Capacity { get; set; } = 2;
    public decimal PricePerNight { get; set; }
    public bool IsAvailable { get; set; } = true;
    public string ImageUrl { get; set; } = string.Empty;
}
