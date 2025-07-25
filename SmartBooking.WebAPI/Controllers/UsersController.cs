using Microsoft.AspNetCore.Mvc;
using SmartBooking.Shared.Dto;
using SmartBooking.Shared;
using SmartBooking.WebAPI.Services.Interfaces;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _users;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService users, ILogger<UsersController> logger)
    {
        _users = users;
        _logger = logger;
    }

    /// <summary>Get all users.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UserDto>), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAllAsync(CancellationToken ct)
    {
        var result = await _users.GetAllAsync(ct);
        if (!result.IsSuccess)
        {
            _logger.LogError("GetAllAsync failed: {Error}", result.ErrorMessage);
            return StatusCode(500, new { result.ErrorMessage });
        }

        return Ok(result.Value);
    }

    /// <summary>Get a user by ID.</summary>
    [HttpGet("{id}", Name = "GetUserById")]
    [ProducesResponseType(typeof(UserDto), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<UserDto>> GetByIdAsync(string id, CancellationToken ct)
    {
        var result = await _users.GetByIdAsync(id, ct);
        if (!result.IsSuccess)
        {
            _logger.LogWarning("GetByIdAsync failed for {Id}: {Error}", id, result.ErrorMessage);
            return NotFound(new { result.ErrorMessage });
        }

        return Ok(result.Value);
    }

    /// <summary>Create a new user.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(UserDto), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<UserDto>> CreateAsync([FromBody] UserCreateDto dto, CancellationToken ct)
    {
        var result = await _users.CreateAsync(dto, ct);
        if (!result.IsSuccess)
        {
            _logger.LogWarning("CreateAsync failed for {Email}: {Error}", dto.Email, result.ErrorMessage);
            return BadRequest(new { result.ErrorMessage });
        }

        var user = result.Value;
        return CreatedAtRoute(routeName: "GetUserById", routeValues: new { id = user.Id }, value: user);
    }

    /// <summary>Update an existing user.</summary>
    [HttpPut("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateAsync(string id, [FromBody] UserEditDto dto, CancellationToken ct)
    {
        var result = await _users.UpdateAsync(id, dto, ct);
        if (!result.IsSuccess)
        {
            _logger.LogWarning("UpdateAsync failed for {Id}: {Error}", id, result.ErrorMessage);
            if (result.ErrorMessage.Contains("not found"))
                return NotFound(new { result.ErrorMessage });
            return BadRequest(new { result.ErrorMessage });
        }

        return NoContent();
    }

    /// <summary>Delete a user.</summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteAsync(string id, CancellationToken ct)
    {
        var result = await _users.DeleteAsync(id, ct);
        if (!result.IsSuccess)
        {
            _logger.LogWarning("DeleteAsync failed for {Id}: {Error}", id, result.ErrorMessage);
            return NotFound(new { result.ErrorMessage });
        }

        return NoContent();
    }
}