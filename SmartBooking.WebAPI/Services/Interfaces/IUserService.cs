using SmartBooking.Shared;
using SmartBooking.Shared.Dto;

namespace SmartBooking.WebAPI.Services.Interfaces;

public interface IUserService
{
    Task<Result<List<UserDto>>> GetAllAsync(CancellationToken ct = default);
    Task<Result<UserDto>> GetByIdAsync(string id, CancellationToken ct = default);
    Task<Result<UserDto>> CreateAsync(UserCreateDto dto, CancellationToken ct = default);
    Task<Result<bool>> UpdateAsync(string id, UserEditDto dto, CancellationToken ct = default);
    Task<Result<bool>> DeleteAsync(string id, CancellationToken ct = default);
}