using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Application.DTOs.Hotels;

public class ChainFilterDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int Count { get; set; }
}