using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using SmartBooking.BlazorUI.Services.Interfaces;
using SmartBooking.Shared;
using SmartBooking.Shared.Dto;
using SmartBooking.Shared.Http.Requests;
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

    public async Task<Result<List<UserDto>>> GetAllAsync()
    {
        try
        {
            await AddAuthHeaderAsync();
            var users = await http.GetFromJsonAsync<List<UserDto>>("users");
            if (users == null)
            {
                logger.LogWarning("No users found.");
                return Result<List<UserDto>>.Failure("No users found");
            }

            return Result<List<UserDto>>.Success(users ?? new List<UserDto>());
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Error retrieving users.");
            return Result<List<UserDto>>.Failure("Error retrieving users");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unexpected error retrieving users.");
            return Result<List<UserDto>>.Failure("Unexpected error retrieving users");
        }
    }

    public async Task<Result<bool>> CreateAsync(UserCreateRequest dto)
    {
        try
        {
            await AddAuthHeaderAsync();
            var response = await http.PostAsJsonAsync("users", dto);
            response.EnsureSuccessStatusCode();
            logger.LogInformation("User created successfully.");
            return Result<bool>.Success(true);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Error creating user.");
            return Result<bool>.Failure("Error creating user");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unexpected error creating user.");
            return Result<bool>.Failure("Unexpected error creating user");
        }
    }

    public async Task<Result<bool>> UpdateAsync(string id, UserEditRequest dto)
    {
        try
        {
            await AddAuthHeaderAsync();
            var response = await http.PutAsJsonAsync($"users/{id}", dto);
            response.EnsureSuccessStatusCode();
            logger.LogInformation("User {Id} updated successfully.", id);
            return Result<bool>.Success(true);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Error updating user {Id}.", id);
            return Result<bool>.Failure("Error updating user");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unexpected error updating user {Id}.", id);
            return Result<bool>.Failure("Unexpected error updating user");
        }
    }

    public async Task<Result<bool>> DeleteAsync(string id)
    {
        try
        {
            await AddAuthHeaderAsync();
            var response = await http.DeleteAsync($"users/{id}");
            response.EnsureSuccessStatusCode();
            logger.LogInformation("User {Id} deleted successfully.", id);
            return Result<bool>.Success(true);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Error deleting user {Id}.", id);
            return Result<bool>.Failure("Error deleting user");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unexpected error deleting user {Id}.", id);
            return Result<bool>.Failure("Unexpected error deleting user");
        }
    }
}