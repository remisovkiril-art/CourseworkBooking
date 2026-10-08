namespace Booking.Application.DTOs.Bookings;

public class BookingDto
{
    public Guid Id { get; set; }

    public Guid RoomId { get; set; }

    public Guid HotelId { get; set; }

    public string HotelName { get; set; } = string.Empty;

    public string HotelAddress { get; set; } = string.Empty;

    public string RoomTitle { get; set; } = string.Empty;

    public DateTime CheckInDate { get; set; }

    public DateTime CheckOutDate { get; set; }

    public int AdultsCount { get; set; }

    public int ChildrenCount { get; set; }

    public string TravelDetails { get; set; } =
        string.Empty;

    public int Nights { get; set; }

    public decimal TotalPrice { get; set; }

    public bool IsPaid { get; set; }

    public string Status { get; set; } =
        string.Empty;

    public DateTime CreatedAt { get; set; }
}