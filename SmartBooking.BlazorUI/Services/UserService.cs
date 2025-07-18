using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using SmartBooking.BlazorUI.Models;
using SmartBooking.BlazorUI.Services.Interfaces;
using SmartBooking.Shared.Dto;
using System.Net.Http.Headers;

public class UserService(HttpClient http, ProtectedLocalStorage storage, ILogger<UserService> logger) : IUserService
{
    private async Task AddAuthHeaderAsync()
    {
        var tokenResult = await storage.GetAsync<string>("authToken");
        if (tokenResult.Success && !string.IsNullOrEmpty(tokenResult.Value))
        {
            http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", tokenResult.Value);
        }
    }

    public async Task<List<UserDto>> GetAllAsync()
    {
        await AddAuthHeaderAsync();
        return await http.GetFromJsonAsync<List<UserDto>>("users")
               ?? new List<UserDto>();
    }

    public async Task<(bool, string)> CreateAsync(UserCreateRequest dto)
    {
        await AddAuthHeaderAsync();
        var resp = await http.PostAsJsonAsync("users", dto);

        if (!resp.IsSuccessStatusCode)
        {
            var errorContent = await resp.Content.ReadAsStringAsync();

            logger.LogError("Failed to create user. Status code: {StatusCode}, Error: {ErrorContent}", resp.StatusCode, errorContent);

            return (false, errorContent);
        }

        return (true, string.Empty);
    }

    public async Task<bool> UpdateAsync(string id, UserEditRequest dto)
    {
        await AddAuthHeaderAsync();
        var resp = await http.PutAsJsonAsync($"users/{id}", dto);
        return resp.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        await AddAuthHeaderAsync();
        var resp = await http.DeleteAsync($"users/{id}");
        return resp.IsSuccessStatusCode;
    }
}