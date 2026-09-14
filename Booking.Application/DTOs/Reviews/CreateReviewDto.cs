namespace Booking.Application.DTOs.Reviews;

public class CreateReviewDto
{
    public Guid HotelId { get; set; }
    public int Rating { get; set; }
    public string Text { get; set; } = string.Empty;
}