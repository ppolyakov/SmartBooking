namespace SmartBooking.WebAPI.Models;

public class SmtpSettings
{
    public string Host { get; init; } = null!;
    public int Port { get; init; }
    public string Username { get; init; } = null!;
    public string Password { get; init; } = null!;
    public string From { get; init; } = null!;
    public string DisplayName { get; init; } = null!;
    public bool UseSsl { get; init; }
}