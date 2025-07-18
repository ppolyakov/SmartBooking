using SmartBooking.BlazorUI.Models;
using SmartBooking.BlazorUI.Services.Interfaces;
using SmartBooking.BlazorUI.Services.Provider;

namespace SmartBooking.BlazorUI.Services;

public class AuthService(HttpClient httpClient, JwtAuthStateProvider jwtAuthStateProvider, ILogger<AuthService> logger) : IAuthService
{
    public async Task<bool> LoginAsync(LoginRequest request)
    {
        var resp = await httpClient.PostAsJsonAsync("auth/login", request);
        if (!resp.IsSuccessStatusCode)
        {
            logger.LogError("Login failed with status code: {StatusCode}", resp.StatusCode);
            return false;
        }

        var result = await resp.Content.ReadFromJsonAsync<LoginResponse>();
        if (result?.Token is null)
        {
            logger.LogError("Login response did not contain a token.");
            return false;
        }

        await jwtAuthStateProvider.MarkUserAsAuthenticated(result.Token);

        return true;
    }

    public async Task<bool> RegisterAsync(RegisterRequest request)
    {
        var resp = await httpClient.PostAsJsonAsync("auth/register", request);

        if (resp.IsSuccessStatusCode)
        {
            return true;
        }
        else
        {
            logger.LogError("Registration failed with status code: {StatusCode}", resp.StatusCode);
            return false;
        }
    }
}