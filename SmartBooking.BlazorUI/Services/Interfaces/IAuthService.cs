using SmartBooking.Shared;
using SmartBooking.Shared.Http.Requests;

namespace SmartBooking.BlazorUI.Services.Interfaces;

public interface IAuthService
{
    Task<Result<bool>> RegisterAsync(RegisterRequest request);
    Task<Result<bool>> LoginAsync(LoginRequest request);
}