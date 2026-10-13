namespace Booking.Domain.Entities;

public class UserFavoriteHotel
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid HotelId { get; set; }
    public Hotel Hotel { get; set; } = null!;
}
