using System.Security.Claims;
using System.Text.Json;

namespace SmartBooking.BlazorUI.Helpers;

public static class JwtParser
{
    public static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var parts = jwt.Split('.');
        if (parts.Length < 2)
            return Enumerable.Empty<Claim>();

        var payload = parts[1];

        // Base64Url decode
        payload = payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=')
                         .Replace('-', '+')
                         .Replace('_', '/');

        var bytes = Convert.FromBase64String(payload);
        var json = System.Text.Encoding.UTF8.GetString(bytes);

        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(json)
                            ?? new Dictionary<string, object>();

        return keyValuePairs.Select(kvp =>
            new Claim(kvp.Key, kvp.Value.ToString() ?? string.Empty));
    }
}