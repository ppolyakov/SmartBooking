using Microsoft.AspNetCore.Mvc;
using SmartBooking.WebAPI.Models;
using SmartBooking.WebAPI.Services.Interfaces;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>Register a new user with User role.</summary>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto, CancellationToken ct)
    {
        var result = await _authService.RegisterAsync(dto, ct);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Registration failed for {Email}: {Error}", dto.Email, result.ErrorMessage);

            return BadRequest(new { result.ErrorMessage });
        }

        return StatusCode(StatusCodes.Status201Created);
    }

    /// <summary>Authenticate and receive a JWT.</summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(TokenResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginDto dto, CancellationToken ct)
    {
        var result = await _authService.LoginAsync(dto, ct);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Invalid login attempt for {Email}: {Error}", dto.Email, result.ErrorMessage);

            return Unauthorized(new { result.ErrorMessage });
        }

        return Ok(new TokenResponseDto(result.Value));
    }
}