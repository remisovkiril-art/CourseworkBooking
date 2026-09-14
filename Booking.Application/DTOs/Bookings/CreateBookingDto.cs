namespace Booking.Application.DTOs.Bookings;

public class CreateBookingDto
{
    public Guid RoomId { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public int AdultsCount { get; set; }
    public int ChildrenCount { get; set; }
    public string TravelDetails { get; set; } = string.Empty;
    public bool IsPaid { get; set; }
}