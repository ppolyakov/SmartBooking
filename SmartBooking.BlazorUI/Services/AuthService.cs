using SmartBooking.BlazorUI.Helpers;
using SmartBooking.BlazorUI.Services.Interfaces;
using SmartBooking.BlazorUI.Services.Provider;
using SmartBooking.Shared;
using SmartBooking.Shared.Http.Requests;
using SmartBooking.Shared.Http.Responses;

namespace SmartBooking.BlazorUI.Services;

public class AuthService(HttpClient httpClient, JwtAuthStateProvider jwtAuthStateProvider, ILogger<AuthService> logger) : IAuthService
{
    public async Task<Result<bool>> LoginAsync(LoginRequest request)
    {
        try 
        {
            var response = await httpClient.PostAsJsonAsync("auth/login", request);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            if (result?.Token is null)
            {
                logger.LogError("Login response did not contain a token.");
                return Result<bool>.Failure("Login failed: No token received.");
            }
            await jwtAuthStateProvider.MarkUserAsAuthenticated(result.Token);
            return Result<bool>.Success(true);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Error during login.");
            return Result<bool>.Failure("Login failed");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unexpected error during login.");
            return Result<bool>.Failure("Unexpected error during login");
        }
    }

    public async Task<Result<bool>> RegisterAsync(RegisterRequest request)
    {
        try 
        {
            var response = await httpClient.PostAsJsonAsync("auth/register", request);
            response.EnsureSuccessStatusCode();
            logger.LogInformation("User registered successfully.");
            return Result<bool>.Success(true);
        }
        catch (HttpRequestException ex) 
        {
            logger.LogError(ex, "Error during registration.");
            return Result<bool>.Failure("Registration failed");
        }
        catch (Exception e) 
        {
            logger.LogError(e, "Unexpected error during registration.");
            return Result<bool>.Failure("Unexpected error during registration");
        }
    }
}