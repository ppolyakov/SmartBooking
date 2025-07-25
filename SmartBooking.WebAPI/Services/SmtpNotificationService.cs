using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using SmartBooking.WebAPI.Models;
using SmartBooking.WebAPI.Services.Interfaces;
using MailKit.Net.Smtp;

namespace SmartBooking.WebAPI.Services;

public class SmtpNotificationService : INotificationService
{
    private readonly SmtpSettings _smtp;
    private readonly ILogger<SmtpNotificationService> _logger;

    public SmtpNotificationService(
        IOptions<SmtpSettings> opts,
        ILogger<SmtpNotificationService> logger)
    {
        _smtp = opts.Value;
        _logger = logger;
    }

    public async Task SendBookingConfirmationAsync(
        string toEmail,
        string toName,
        Guid bookingId,
        DateTime startTime,
        string serviceTitle,
        CancellationToken ct = default)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_smtp.DisplayName, _smtp.From));
        message.To.Add(new MailboxAddress(toName, toEmail));
        message.Subject = "Ваше бронирование подтверждено";

        message.Body = new BodyBuilder
        {
            HtmlBody = $@"
                <p>Здравствуйте, {toName}!</p>
                <p>Ваша услуга <strong>{serviceTitle}</strong> на {startTime:dd.MM.yyyy HH:mm} подтверждена.</p>
                <p>ID брони: <code>{bookingId}</code></p>"
        }.ToMessageBody();

        try
        {
            using var client = new SmtpClient();
            var socketOpts = _smtp.UseSsl
                ? SecureSocketOptions.SslOnConnect
                : SecureSocketOptions.StartTls;

            await client.ConnectAsync(_smtp.Host, _smtp.Port, socketOpts, ct);
            await client.AuthenticateAsync(_smtp.Username, _smtp.Password, ct);
            await client.SendAsync(message, ct);
            await client.DisconnectAsync(true, ct);

            _logger.LogInformation("Confirmation email sent to {Email}", toEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending confirmation email to {Email}", toEmail);
        }
    }
}