using Microsoft.AspNetCore.Mvc;
using SmartBooking.Shared.Dto;
using SmartBooking.WebAPI.Services.Interfaces;

[ApiController]
[Route("api/[controller]")]
public class UsersController(IUserService userService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<UserDto>>> GetAll()
    {
        var result = await userService.GetAll();
        if (!result.IsSuccess)
            return BadRequest(result.ErrorMessage);

        var list = result.Value;
        return Ok(list);
    }

    [HttpPost]
    public async Task<ActionResult<UserDto>> Create(UserCreateDto dto)
    {
        if (dto == null)
        {
            return BadRequest(new { Error = "Invalid user data." });
        }
        var result = await userService.Create(dto);
        if (!result.IsSuccess)
        {
            return BadRequest(new { Error = result.ErrorMessage });
        }
        var user = result.Value;
        return Ok(user);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, UserEditDto dto)
    {
        if (string.IsNullOrEmpty(id) || dto == null)
        {
            return BadRequest(new { Error = "Invalid user ID or data." });
        }
        var result = await userService.Update(id, dto);
        if (!result.IsSuccess)
        {
            return BadRequest(new { Error = result.ErrorMessage });
        }
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return BadRequest(new { Error = "Invalid user ID." });
        }
        var result = await userService.Delete(id);
        if (!result.IsSuccess)
        {
            return BadRequest(new { Error = result.ErrorMessage });
        }
        return NoContent();
    }
}