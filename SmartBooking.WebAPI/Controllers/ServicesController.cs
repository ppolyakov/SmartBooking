using Microsoft.AspNetCore.Mvc;
using SmartBooking.Shared.Dto;
using SmartBooking.WebAPI.Services.Interfaces;

[ApiController]
[Route("api/[controller]")]
public class ServicesController : ControllerBase
{
    private readonly IServiceService _svc;
    private readonly ILogger<ServicesController> _logger;

    public ServicesController(IServiceService svc, ILogger<ServicesController> logger)
    {
        _svc = svc;
        _logger = logger;
    }

    /// <summary>Get all services (with no slots).</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ServiceDto>), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<IEnumerable<ServiceDto>>> GetAllAsync(CancellationToken ct)
    {
        var result = await _svc.GetAllAsync(ct);
        if (!result.IsSuccess)
        {
            _logger.LogError("GetAllAsync failed: {Error}", result.ErrorMessage);
            return StatusCode(500, new { result.ErrorMessage });
        }
        return Ok(result.Value);
    }

    /// <summary>Create a new service.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ServiceDto), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<ServiceDto>> CreateAsync(
        [FromBody] CreateServiceDto dto,
        CancellationToken ct)
    {
        var result = await _svc.CreateAsync(dto, ct);
        if (!result.IsSuccess)
        {
            _logger.LogWarning("CreateAsync failed: {Error}", result.ErrorMessage);
            return BadRequest(new { result.ErrorMessage });
        }
        var created = result.Value;
        return CreatedAtAction(nameof(GetAllAsync), null, created);
    }

    /// <summary>Get all services with their slots and client emails.</summary>
    [HttpGet("with-slots")]
    [ProducesResponseType(typeof(IEnumerable<ServiceWithSlotsDto>), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<IEnumerable<ServiceWithSlotsDto>>> GetWithSlotsAsync(CancellationToken ct)
    {
        var result = await _svc.GetWithSlotsAsync(ct);
        if (!result.IsSuccess)
        {
            _logger.LogError("GetWithSlotsAsync failed: {Error}", result.ErrorMessage);
            return StatusCode(500, new { result.ErrorMessage });
        }
        return Ok(result.Value);
    }
}