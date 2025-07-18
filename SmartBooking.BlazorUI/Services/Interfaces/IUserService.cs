using SmartBooking.BlazorUI.Models;
using SmartBooking.Shared.Dto;

namespace SmartBooking.BlazorUI.Services.Interfaces;

public interface IUserService
{
    Task<List<UserDto>> GetAllAsync();
    Task<(bool, string)> CreateAsync(UserCreateRequest dto);
    Task<bool> UpdateAsync(string id, UserEditRequest dto);
    Task<bool> DeleteAsync(string id);
}