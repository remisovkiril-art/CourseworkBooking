using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Application.DTOs.Reviews;

public class RatingFilterDto
{
    public int MinRating { get; set; }

    public int Count { get; set; }
}