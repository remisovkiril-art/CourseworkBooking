using Booking.Application.DTOs.Reviews;
using Booking.Application.DTOs.Stars;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Application.DTOs.Hotels;

public class HotelFiltersDto
{
    public List<RatingFilterDto> Ratings { get; set; } = [];

    public List<StarsFilterDto> Stars { get; set; } = [];

    public List<HotelTypeFilterDto> Types { get; set; } = [];

    public List<ChainFilterDto> Chains { get; set; } = [];

    public List<AmenityFilterDto> Amenities { get; set; } = [];
}