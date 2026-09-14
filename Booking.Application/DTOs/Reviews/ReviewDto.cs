namespace Booking.Application.DTOs.Reviews;

public class ReviewDto
{
    public Guid Id { get; set; }
    public Guid HotelId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public int Rating { get; set; }
    public DateTime CreatedAt { get; set; }
}