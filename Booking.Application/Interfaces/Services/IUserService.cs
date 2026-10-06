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
        string? avatarUrl,
        CancellationToken cancellationToken);

    Task<TravelPreferencesDto> GetTravelPreferencesAsync(
        Guid userId,
        CancellationToken cancellationToken);

    Task SaveTravelPreferencesAsync(
        Guid userId,
        TravelPreferencesDto dto,
        CancellationToken cancellationToken);

    Task<NewsletterDto?> GetNewsletterAsync(
        Guid userId,
        CancellationToken cancellationToken);

    Task SaveNewsletterAsync(
        Guid userId,
        NewsletterDto dto,
        CancellationToken cancellationToken);
}