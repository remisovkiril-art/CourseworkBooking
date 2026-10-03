using Booking.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Application.DTOs.Hotels;

public class HotelSearchDto
{
    public string? Search { get; set; }

    public int Adults { get; set; }
    public int Children { get; set; }
    public int Rooms { get; set; }

    public DateTime? CheckIn { get; set; }
    public DateTime? CheckOut { get; set; }

    public int? MinRating { get; set; }
    public int? Stars { get; set; }

    public HotelType[]? Types { get; set; }

    public int[]? ChainIds { get; set; }

    public string[]? Amenities { get; set; }

    public string? Near { get; set; }

    public string? Sort { get; set; }

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}