using SmartBooking.Shared;
using SmartBooking.WebAPI.Models;

namespace SmartBooking.WebAPI.Services.Interfaces;

public interface IAuthService
{
    Task<Result<bool>> RegisterAsync(RegisterDto registerDto);
    Task<Result<string>> LoginAsync(LoginDto loginDto);
}