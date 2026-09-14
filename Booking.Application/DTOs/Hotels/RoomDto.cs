namespace Booking.Application.DTOs.Hotels;

public class RoomDto
{
    public Guid Id { get; set; }
    public Guid HotelId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string BedType { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public decimal PricePerNight { get; set; }
    public bool IsAvailable { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
}