using Microsoft.AspNetCore.Identity;
using SmartBooking.Infrastructure.Persistence;
using SmartBooking.Shared;
using SmartBooking.Shared.Dto;
using SmartBooking.WebAPI.Services.Interfaces;

namespace SmartBooking.WebAPI.Services;

public class UserService(UserManager<ApplicationUser> um, RoleManager<IdentityRole> rm, ILogger<UserService> logger) : IUserService
{
    public async Task<Result<UserDto>> Create(UserCreateDto dto)
    {
        try
        {
            var user = new ApplicationUser { UserName = dto.Email, Email = dto.Email };
            var res = await um.CreateAsync(user, dto.Password);
            if (!res.Succeeded)
            {
                logger.LogError("User creation failed: {Errors}", string.Join(", ", res.Errors.Select(e => e.Description)));
                return Result<UserDto>.Failure("User creation failed: " + string.Join(", ", res.Errors.Select(e => e.Description)));
            }

            await um.AddToRoleAsync(user, "User");
            if (dto.IsAdmin) await um.AddToRoleAsync(user, "Admin");

            var roles = await um.GetRolesAsync(user);
            return Result<UserDto>.Success(new UserDto { Id = user.Id, Email = user.Email!, Roles = roles.ToList() });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating user.");
            return Result<UserDto>.Failure($"Error creating user: {ex.Message}");
        }
    }

    public async Task<Result<bool>> Delete(string id)
    {
        try
        {
            var user = await um.FindByIdAsync(id);
            if (user == null)
            {
                logger.LogWarning("User with ID {Id} not found.", id);
                return Result<bool>.Failure($"User with ID {id} not found.");
            }
            await um.DeleteAsync(user);

            logger.LogInformation("User with ID {Id} deleted successfully.", id);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting user.");
            return Result<bool>.Failure($"Error deleting user: {ex.Message}");
        }
    }

    public async Task<Result<List<UserDto>>> GetAll()
    {
        try
        {
            var users = um.Users.ToList();
            var list = new List<UserDto>();
            foreach (var u in users)
            {
                var roles = await um.GetRolesAsync(u);
                list.Add(new UserDto { Id = u.Id, Email = u.Email!, Roles = roles.ToList() });
            }
            return Result<List<UserDto>>.Success(list);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving users.");
            return Result<List<UserDto>>.Failure($"Error retrieving users: {ex.Message}");
        }
    }

    public async Task<Result<bool>> Update(string id, UserEditDto dto)
    {
        try
        {
            var user = await um.FindByIdAsync(id);
            if (user == null)
            {
                logger.LogWarning("User with ID {Id} not found.", id);
                return Result<bool>.Failure($"User with ID {id} not found.");
            }

            user.Email = dto.Email;
            user.UserName = dto.Email;
            await um.UpdateAsync(user);

            var roles = await um.GetRolesAsync(user);
            if (dto.IsAdmin && !roles.Contains("Admin"))
                await um.AddToRoleAsync(user, "Admin");
            if (!dto.IsAdmin && roles.Contains("Admin"))
                await um.RemoveFromRoleAsync(user, "Admin");

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating user.");
            return Result<bool>.Failure($"Error updating user: {ex.Message}");
        }
    }
}