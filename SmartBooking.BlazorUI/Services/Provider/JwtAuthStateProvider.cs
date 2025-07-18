using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using SmartBooking.BlazorUI.Helpers;
using System.Security.Claims;

namespace SmartBooking.BlazorUI.Services.Provider;

public class JwtAuthStateProvider : AuthenticationStateProvider
{
    private readonly ProtectedLocalStorage _storage;
    public JwtAuthStateProvider(ProtectedLocalStorage storage)
        => _storage = storage;

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var saved = await _storage.GetAsync<string>("authToken");
        ClaimsIdentity identity;

        if (saved.Success && !string.IsNullOrWhiteSpace(saved.Value))
        {
            var claims = JwtParser.ParseClaimsFromJwt(saved.Value);
            identity = new ClaimsIdentity(claims, "jwt");
        }
        else
        {
            identity = new ClaimsIdentity();
        }

        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    public async Task MarkUserAsAuthenticated(string token)
    {
        await _storage.SetAsync("authToken", token);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task MarkUserAsLoggedOut()
    {
        await _storage.DeleteAsync("authToken");
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}