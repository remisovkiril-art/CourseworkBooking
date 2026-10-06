namespace Booking.Application.DTOs.Hotels;

public class HotelSearchResultDto
{
    public int TotalCount { get; set; }

    public List<HotelDto> Hotels { get; set; } =
        new();
}