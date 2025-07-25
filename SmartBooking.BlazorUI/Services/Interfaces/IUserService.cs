using SmartBooking.Shared;
using SmartBooking.Shared.Dto;
using SmartBooking.Shared.Http.Requests;

namespace SmartBooking.BlazorUI.Services.Interfaces;

public interface IUserService
{
    Task<Result<List<UserDto>>> GetAllAsync();
    Task<Result<bool>> CreateAsync(UserCreateDto dto);
    Task<Result<bool>> UpdateAsync(string id, UserEditDto dto);
    Task<Result<bool>> DeleteAsync(string id);
}