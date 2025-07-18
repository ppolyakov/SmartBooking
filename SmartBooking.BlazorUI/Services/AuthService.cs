using SmartBooking.BlazorUI.Models;
using SmartBooking.BlazorUI.Services.Interfaces;
using SmartBooking.BlazorUI.Services.Provider;

namespace SmartBooking.BlazorUI.Services;

public class AuthService : IAuthService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly JwtAuthStateProvider _jwtAuthStateProvider;
    private readonly HttpClient _http;

    public AuthService(IHttpClientFactory httpClientFactory, JwtAuthStateProvider jwtAuthStateProvider)
    {
        _httpClientFactory = httpClientFactory;
        _jwtAuthStateProvider = jwtAuthStateProvider;
        _http = _httpClientFactory.CreateClient("SmartBookingAPI");
    }

    public async Task<bool> LoginAsync(LoginRequest request)
    {
        var resp = await _http.PostAsJsonAsync("auth/login", request);
        if (!resp.IsSuccessStatusCode)
        {
            return false;
        }

        var result = await resp.Content.ReadFromJsonAsync<LoginResponse>();
        if (result?.Token is null)
        {
            return false;
        }

        await _jwtAuthStateProvider.MarkUserAsAuthenticated(result.Token);

        return true;
    }

    public async Task<bool> RegisterAsync(RegisterRequest request)
    {
        var resp = await _http.PostAsJsonAsync("auth/register", request);

        if (resp.IsSuccessStatusCode)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}