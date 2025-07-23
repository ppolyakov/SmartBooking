using Microsoft.AspNetCore.Mvc;
using SmartBooking.WebAPI.Models;
using SmartBooking.WebAPI.Services.Interfaces;

namespace SmartBooking.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    /// <summary>
    /// Registers a new user and assigns them the Admin role.
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        if (dto == null)
        {
            return BadRequest(new { Error = "Invalid registration data." });
        }

        var result = await authService.RegisterAsync(dto);
        if (!result.IsSuccess)
        {
            return BadRequest(new { Error = result.ErrorMessage });
        }
        return Ok(new { Message = "Registration successful." });
    }

    /// <summary>
    /// Authenticates a user and returns a JWT token.
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        if (dto == null)
        {
            return BadRequest(new { Error = "Invalid login data." });
        }

        var result = await authService.LoginAsync(dto);

        if (!result.IsSuccess)
        {
            return BadRequest(new { Error = result.ErrorMessage });
        }

        return Ok(new { Token = result.Value });
    }
}