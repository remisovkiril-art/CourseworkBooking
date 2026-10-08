using Booking.Application.Interfaces.Services;
using Microsoft.Extensions.Hosting;

namespace Booking.Infrastructure.Services;

public class ImageService : IImageService
{
    private readonly IHostEnvironment _environment;

    public ImageService(
        IHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<string> SaveHotelImageAsync(
        Stream stream,
        string fileName,
        CancellationToken cancellationToken)
    {
        string folder =
            Path.Combine(
                _environment.ContentRootPath,
                "wwwroot",
                "hotels");

        Directory.CreateDirectory(folder);

        string extension =
            Path.GetExtension(fileName)
                .ToLowerInvariant();

        if (string.IsNullOrEmpty(extension))
        {
            extension = ".jpg";
        }

        string newFileName =
            $"{Guid.NewGuid()}{extension}";

        string path =
            Path.Combine(
                folder,
                newFileName);

        await using FileStream fileStream =
            new FileStream(
                path,
                FileMode.Create);

        await stream.CopyToAsync(
            fileStream,
            cancellationToken);

        return $"/hotels/{newFileName}";
    }

    public async Task<string> SaveUserAvatarAsync(
        Stream stream,
        string fileName,
        CancellationToken cancellationToken)
    {
        string[] allowedExtensions =
        {
            ".jpg",
            ".jpeg",
            ".png",
            ".webp"
        };

        string extension =
            Path.GetExtension(fileName)
                .ToLowerInvariant();

        if (!allowedExtensions.Contains(extension))
        {
            throw new Exception(
                "Unsupported image format.");
        }

        string folder =
            Path.Combine(
                _environment.ContentRootPath,
                "wwwroot",
                "avatars");

        Directory.CreateDirectory(folder);

        string newFileName =
            $"{Guid.NewGuid()}{extension}";

        string path =
            Path.Combine(
                folder,
                newFileName);

        await using FileStream fileStream =
            new FileStream(
                path,
                FileMode.Create);

        await stream.CopyToAsync(
            fileStream,
            cancellationToken);

        return $"/avatars/{newFileName}";
    }

    public async Task<string> SaveRoomImageAsync(
        Stream stream,
        string fileName,
        CancellationToken cancellationToken)
    {
        string[] allowedExtensions =
        {
            ".jpg",
            ".jpeg",
            ".png",
            ".webp"
        };

        string extension =
            Path.GetExtension(fileName)
                .ToLowerInvariant();

        if (!allowedExtensions.Contains(extension))
        {
            throw new Exception(
                "Unsupported image format.");
        }

        string folder =
            Path.Combine(
                _environment.ContentRootPath,
                "wwwroot",
                "rooms");

        Directory.CreateDirectory(folder);

        string newFileName =
            $"{Guid.NewGuid()}{extension}";

        string path =
            Path.Combine(
                folder,
                newFileName);

        await using FileStream fileStream =
            new FileStream(
                path,
                FileMode.Create);

        await stream.CopyToAsync(
            fileStream,
            cancellationToken);

        return $"/rooms/{newFileName}";
    }
}