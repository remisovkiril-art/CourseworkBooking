using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Domain.Entities;

public class Review
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public string Text { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public Guid UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;

    public Guid HotelId { get; set; }

    [ForeignKey(nameof(HotelId))]
    public Hotel Hotel { get; set; } = null!;
}