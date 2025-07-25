using SmartBooking.Shared;
using SmartBooking.WebAPI.Models;

namespace SmartBooking.WebAPI.Services.Interfaces;

public interface IAuthService
{
    Task<Result<bool>> RegisterAsync(RegisterDto registerDto, CancellationToken ct = default);
    Task<Result<string>> LoginAsync(LoginDto loginDto, CancellationToken ct = default);
}