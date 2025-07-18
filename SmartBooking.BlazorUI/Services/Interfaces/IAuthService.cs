using SmartBooking.BlazorUI.Models;

namespace SmartBooking.BlazorUI.Services.Interfaces;

public interface IAuthService
{
    Task<bool> RegisterAsync(RegisterRequest request);
    Task<bool> LoginAsync(LoginRequest request);
}