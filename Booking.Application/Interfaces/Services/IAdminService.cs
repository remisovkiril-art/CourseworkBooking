using Booking.Application.DTOs.Auth;

namespace Booking.Application.Interfaces.Services;

public interface IAdminService
{
    Task SetFirstAdminAsync(
        Guid userId,
        CancellationToken cancellationToken);

    Task<UserReadDto> CreateModeratorAsync(
        Guid adminUserId,
        CreateModeratorDto dto,
        CancellationToken cancellationToken);

    Task DeleteReviewAsync(
        Guid userId,
        Guid reviewId,
        CancellationToken cancellationToken);
}