namespace Booking.Application.Interfaces.Services;

public interface IImageService
{
    Task<string> SaveHotelImageAsync(
        Stream stream,
        string fileName,
        CancellationToken cancellationToken);

    Task<string> SaveUserAvatarAsync(
     Stream stream,
     string fileName,
     CancellationToken cancellationToken);
}
