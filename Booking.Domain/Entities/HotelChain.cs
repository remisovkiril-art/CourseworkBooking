using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Domain.Entities;

public class HotelChain
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public ICollection<Hotel> Hotels { get; set; } = new List<Hotel>();
}