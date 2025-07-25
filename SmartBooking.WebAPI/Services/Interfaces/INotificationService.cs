namespace SmartBooking.WebAPI.Services.Interfaces;

public interface INotificationService
{
    Task SendBookingConfirmationAsync(string toEmail, string toName, Guid bookingId, DateTime startTime, string serviceTitle, CancellationToken ct = default);
}