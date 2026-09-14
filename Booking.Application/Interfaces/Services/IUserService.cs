using Booking.Application.DTOs.Auth;

namespace Booking.Application.Interfaces.Services;

public interface IUserService
{
    Task<UpdateUserDto?> GetAsync(
        Guid userId,
        CancellationToken cancellationToken);

    Task UpdateAsync(
        Guid userId,
        UpdateUserDto dto,
        CancellationToken cancellationToken);
}
