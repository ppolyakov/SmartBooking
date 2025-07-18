using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmartBooking.Infrastructure.Identity;
using SmartBooking.Shared.Dto;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _um;
    private readonly RoleManager<IdentityRole> _rm;

    public UsersController(UserManager<ApplicationUser> um, RoleManager<IdentityRole> rm)
    {
        _um = um;
        _rm = rm;
    }

    [HttpGet]
    public async Task<ActionResult<List<UserDto>>> GetAll()
    {
        var users = _um.Users.ToList();
        var list = new List<UserDto>();
        foreach (var u in users)
        {
            var roles = await _um.GetRolesAsync(u);
            list.Add(new UserDto { Id = u.Id, Email = u.Email!, Roles = roles.ToList() });
        }
        return Ok(list);
    }

    [HttpPost]
    public async Task<ActionResult<UserDto>> Create(UserCreateDto dto)
    {
        var user = new ApplicationUser { UserName = dto.Email, Email = dto.Email };
        var res = await _um.CreateAsync(user, dto.Password);
        if (!res.Succeeded) return BadRequest(res.Errors);

        await _um.AddToRoleAsync(user, "User");
        if (dto.IsAdmin) await _um.AddToRoleAsync(user, "Admin");

        var roles = await _um.GetRolesAsync(user);
        return Ok(new UserDto { Id = user.Id, Email = user.Email!, Roles = roles.ToList() });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, UserEditDto dto)
    {
        var user = await _um.FindByIdAsync(id);
        if (user == null) return NotFound();

        user.Email = dto.Email;
        user.UserName = dto.Email;
        await _um.UpdateAsync(user);

        var roles = await _um.GetRolesAsync(user);
        if (dto.IsAdmin && !roles.Contains("Admin"))
            await _um.AddToRoleAsync(user, "Admin");
        if (!dto.IsAdmin && roles.Contains("Admin"))
            await _um.RemoveFromRoleAsync(user, "Admin");

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var user = await _um.FindByIdAsync(id);
        if (user == null) return NotFound();
        await _um.DeleteAsync(user);
        return NoContent();
    }
}