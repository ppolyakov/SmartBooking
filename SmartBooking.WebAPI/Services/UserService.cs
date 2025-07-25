using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartBooking.Infrastructure.Persistence;
using SmartBooking.Shared;
using SmartBooking.Shared.Dto;
using SmartBooking.WebAPI.Services.Interfaces;

public class UserService(UserManager<ApplicationUser> um, RoleManager<IdentityRole> rm, AppDbContext db, ILogger<UserService> logger) : IUserService
{
    private readonly UserManager<ApplicationUser> _um = um;
    private readonly RoleManager<IdentityRole> _rm = rm;
    private readonly ILogger<UserService> _logger = logger;
    private readonly AppDbContext _db = db;

    public async Task<Result<List<UserDto>>> GetAllAsync(CancellationToken ct = default)
    {
        try
        {
            var users = await _um.Users
                .AsNoTracking()
                .ToListAsync(ct);

            var list = new List<UserDto>(users.Count);
            foreach (var u in users)
            {
                var roles = await _um.GetRolesAsync(u);

                var userDto = new UserDto
                {
                    Id = u.Id,
                    Email = u.Email,
                    Roles = roles.ToList(),
                };
                list.Add(userDto);
            }

            _logger.LogInformation("Retrieved {Count} users.", list.Count);
            return Result<List<UserDto>>.Success(list);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetAllAsync");
            return Result<List<UserDto>>.Failure("Could not retrieve users.");
        }
    }

    public async Task<Result<UserDto>> GetByIdAsync(string id, CancellationToken ct = default)
    {
        var u = await _um.FindByIdAsync(id);
        if (u == null)
            return Result<UserDto>.Failure("User not found.");

        var roles = await _um.GetRolesAsync(u);

        var userDto = new UserDto
        {
            Id = u.Id,
            Email = u.Email,
            Roles = roles.ToList(),
        };
        return Result<UserDto>.Success(userDto);
    }

    public async Task<Result<UserDto>> CreateAsync(UserCreateDto dto, CancellationToken ct = default)
    {
        if (await _um.FindByEmailAsync(dto.Email) != null)
            return Result<UserDto>.Failure("Email already taken.");

        var user = new ApplicationUser { UserName = dto.Email, Email = dto.Email };
        var res = await _um.CreateAsync(user, dto.Password);
        if (!res.Succeeded)
        {
            var errors = string.Join("; ", res.Errors.Select(e => e.Description));
            _logger.LogError("CreateAsync errors: {Errors}", errors);
            return Result<UserDto>.Failure(errors);
        }

        await _um.AddToRoleAsync(user, "User");
        if (dto.IsAdmin)
            await _um.AddToRoleAsync(user, "Admin");

        _logger.LogInformation("Created user {Email}.", user.Email);
        var roles = await _um.GetRolesAsync(user);
        var userDto = new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            Roles = roles.ToList(),
        };
        return Result<UserDto>.Success(userDto);
    }

    public async Task<Result<bool>> UpdateAsync(string id, UserEditDto dto, CancellationToken ct = default)
    {
        var user = await _um.FindByIdAsync(id);
        if (user == null)
            return Result<bool>.Failure("User not found.");

        user.Email = dto.Email;
        user.UserName = dto.Email;
        var upd = await _um.UpdateAsync(user);
        if (!upd.Succeeded)
        {
            var errors = string.Join("; ", upd.Errors.Select(e => e.Description));
            _logger.LogError("UpdateAsync errors: {Errors}", errors);
            return Result<bool>.Failure(errors);
        }

        var roles = await _um.GetRolesAsync(user);
        if (dto.IsAdmin && !roles.Contains("Admin"))
            await _um.AddToRoleAsync(user, "Admin");
        if (!dto.IsAdmin && roles.Contains("Admin"))
            await _um.RemoveFromRoleAsync(user, "Admin");

        _logger.LogInformation("Updated user {Id}.", id);
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> DeleteAsync(string id, CancellationToken ct = default)
    {
        var user = await _um.FindByIdAsync(id);
        if (user == null)
            return Result<bool>.Failure("User not found.");

        var res = await _um.DeleteAsync(user);
        if (!res.Succeeded)
        {
            var errors = string.Join("; ", res.Errors.Select(e => e.Description));
            _logger.LogError("DeleteAsync errors: {Errors}", errors);
            return Result<bool>.Failure(errors);
        }

        _logger.LogInformation("Deleted user {Id}.", id);
        return Result<bool>.Success(true);
    }
}