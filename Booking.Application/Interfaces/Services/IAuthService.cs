using Booking.Application.DTOs.Auth;

namespace Booking.Application.Interfaces.Services;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(
        RegisterDto dto,
        CancellationToken cancellationToken);

    Task<AuthResponseDto> LoginAsync(
        LoginDto dto,
        CancellationToken cancellationToken);

    Task<AuthResponseDto> VerifyAsync(
        VerifyCodeDto dto,
        CancellationToken cancellationToken);
}
