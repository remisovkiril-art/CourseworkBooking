using Booking.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Booking.Application.DTOs.Reviews;

public class CreateReviewDto
{
    public Guid HotelId { get; set; }

    [Range(1, 10, ErrorMessage = "Facilities rating must be between 1 and 10.")]
    public double Facilities { get; set; }

    [Range(1, 10, ErrorMessage = "Staff rating must be between 1 and 10.")]
    public double Staff { get; set; }

    [Range(1, 10, ErrorMessage = "Cleanliness rating must be between 1 and 10.")]
    public double Cleanliness { get; set; }

    [Range(1, 10, ErrorMessage = "Comfort rating must be between 1 and 10.")]
    public double Comfort { get; set; }

    [Range(1, 10, ErrorMessage = "Location rating must be between 1 and 10.")]
    public double Location { get; set; }

    [Range(1, 10, ErrorMessage = "Value for money rating must be between 1 and 10.")]
    public double ValueForMoney { get; set; }

    [Required(ErrorMessage = "Review text is required.")]
    public string Text { get; set; } = string.Empty;
}