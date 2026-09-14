using System.Net;
using System.Net.Mail;
using Booking.Application.Interfaces.Services;
using Booking.Application.Settings;

namespace Booking.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;

    public EmailService(EmailSettings settings)
    {
        _settings = settings;
    }

    public async Task SendVerificationCodeAsync(
        string email,
        string code,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_settings.Host) ||
            string.IsNullOrWhiteSpace(_settings.UserName) ||
            string.IsNullOrWhiteSpace(_settings.Password) ||
            string.IsNullOrWhiteSpace(_settings.From))
        {
            return;
        }

        using var message = new MailMessage(
            _settings.From,
            email,
            "Hotel for you. verification code",
            $"Your verification code is: {code}");

        using var client = new SmtpClient(_settings.Host, _settings.Port)
        {
            EnableSsl = _settings.EnableSsl,
            Credentials = new NetworkCredential(
                _settings.UserName,
                _settings.Password)
        };

        await client.SendMailAsync(message, cancellationToken);
    }
}