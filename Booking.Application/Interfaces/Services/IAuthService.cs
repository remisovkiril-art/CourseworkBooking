using Booking.Application.DTOs.Auth;

namespace Booking.Application.Interfaces.Services;

public interface IAuthService
{
    Task<VerificationRequiredDto> RegisterAsync(
        RegisterDto dto,
        CancellationToken cancellationToken);

    Task<VerificationRequiredDto> LoginAsync(
        LoginDto dto,
        CancellationToken cancellationToken);

    Task<AuthResponseDto> VerifyAsync(
        VerifyCodeDto dto,
        CancellationToken cancellationToken);

    Task<AuthResponseDto> GoogleLoginAsync(
        string email,
        string name,
        CancellationToken cancellationToken);

    Task<VerificationRequiredDto> ForgotPasswordAsync(
        ForgotPasswordDto dto,
        CancellationToken cancellationToken);

    Task ResetPasswordAsync(
        ResetPasswordDto dto,
        CancellationToken cancellationToken);
}