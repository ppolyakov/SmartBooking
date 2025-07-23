using SmartBooking.Shared;
using SmartBooking.Shared.Dto;

namespace SmartBooking.WebAPI.Services.Interfaces;

public interface IUserService
{
    Task<Result<List<UserDto>>> GetAll();
    Task<Result<UserDto>> Create(UserCreateDto dto);
    Task<Result<bool>> Update(string id, UserEditDto dto);
    Task<Result<bool>> Delete(string id);
}