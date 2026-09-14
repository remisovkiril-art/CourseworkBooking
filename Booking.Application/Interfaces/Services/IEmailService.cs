namespace Booking.Application.Interfaces.Services;

public interface IEmailService
{
    Task SendVerificationCodeAsync(
        string email,
        string code,
        CancellationToken cancellationToken);
}
