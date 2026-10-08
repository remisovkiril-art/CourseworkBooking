namespace Booking.Application.DTOs.Hotels;

public class HotelSearchResultDto
{
    public int TotalCount { get; set; }
    public int Total { get; set; }
    public List<HotelDto> Hotels { get; set; } = new();
    public int Page { get; set; }
    public int PageSize { get; set; }
}
