using Booking.Application.Interfaces.Services;
using Microsoft.Extensions.Hosting;

namespace Booking.Infrastructure.Services;

public class ImageService : IImageService
{
    private readonly IHostEnvironment _environment;

    public ImageService(IHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<string> SaveHotelImageAsync(
        Stream stream,
        string fileName,
        CancellationToken cancellationToken)
    {
        var folder = Path.Combine(
            _environment.ContentRootPath,
            "wwwroot",
            "hotels");

        Directory.CreateDirectory(folder);

        var extension = Path.GetExtension(fileName);

        if (string.IsNullOrEmpty(extension))
        {
            extension = ".jpg";
        }

        var newFileName =
            $"{Guid.NewGuid()}{extension}";

        var path = Path.Combine(
            folder,
            newFileName);

        await using var fileStream = new FileStream(
            path,
            FileMode.Create);

        await stream.CopyToAsync(
            fileStream,
            cancellationToken);

        return $"/hotels/{newFileName}";
    }
}