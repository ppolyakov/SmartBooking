namespace SmartBooking.WebAPI.Models;

public record JwtSettings
{
    public string Key { get; init; } = null!;
    public string Issuer { get; init; } = null!;
    public string Audience { get; init; } = null!;
    public int ExpiresMinutes { get; init; } = 60;
}