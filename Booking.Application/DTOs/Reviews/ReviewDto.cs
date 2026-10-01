using Booking.Domain.Entities;

namespace Booking.Application.DTOs.Reviews;

public class ReviewDto
{
    public Guid Id { get; set; }
    public Guid HotelId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string? AuthorAvatarUrl { get; set; }
    public string Text { get; set; } = string.Empty;
    public double Rating { get; set; }
    public double Facilities { get; set; }
    public double Staff { get; set; }
    public double Cleanliness { get; set; }
    public double Comfort { get; set; }
    public double Location { get; set; }
    public double ValueForMoney { get; set; }
    public DateTime CreatedAt { get; set; }
}