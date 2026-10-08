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
            throw new InvalidOperationException(
                "Email settings are not configured.");
        }

        using MailMessage message = new MailMessage();

        message.From = new MailAddress(
            _settings.From);

        message.To.Add(
            new MailAddress(email));

        message.Subject =
            "Hotel for you. Verification code";

        message.Body =
            $"Your verification code is: {code}\n\n" +
            "The code is valid for 10 minutes.";

        using SmtpClient client =
            new SmtpClient(
                _settings.Host,
                _settings.Port);

        client.EnableSsl = _settings.EnableSsl;

        client.Credentials =
            new NetworkCredential(
                _settings.UserName,
                _settings.Password);

        await client.SendMailAsync(
            message,
            cancellationToken);
    }
}