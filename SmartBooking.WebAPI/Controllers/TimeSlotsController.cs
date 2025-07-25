using Microsoft.AspNetCore.Mvc;
using SmartBooking.Shared.Dto;
using SmartBooking.WebAPI.Services.Interfaces;

[ApiController]
[Route("api/[controller]")]
public class TimeSlotsController : ControllerBase
{
    private readonly ITimeSlotsService _svc;
    private readonly ILogger<TimeSlotsController> _logger;

    public TimeSlotsController(ITimeSlotsService svc, ILogger<TimeSlotsController> logger)
    {
        _svc = svc;
        _logger = logger;
    }

    /// <summary>Return all time slots</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TimeSlotDto>), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<IEnumerable<TimeSlotDto>>> GetAllAsync(CancellationToken ct)
    {
        var result = await _svc.GetAllAsync(ct);
        if (!result.IsSuccess)
        {
            _logger.LogError("GetAllAsync failed: {Error}", result.ErrorMessage);
            return StatusCode(500, new { result.ErrorMessage });
        }

        _logger.LogInformation("Loaded {Count} time slots", result.Value.Count());
        return Ok(result.Value);
    }

    /// <summary>Generate time slots</summary>
    [HttpPost("generate")]
    [ProducesResponseType(typeof(IEnumerable<TimeSlotDto>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<IEnumerable<TimeSlotDto>>> GenerateAsync([FromQuery] Guid serviceId, [FromQuery] DateTime date, CancellationToken ct)
    {
        if (serviceId == Guid.Empty || date == default)
            return BadRequest(new { Error = "Invalid service ID or date." });

        var result = await _svc.GenerateAsync(serviceId, date, ct);
        if (!result.IsSuccess)
        {
            _logger.LogWarning("GenerateAsync failed for service {ServiceId} on {Date}: {Error}",
                serviceId, date.Date, result.ErrorMessage);
            return BadRequest(new { result.ErrorMessage });
        }

        _logger.LogInformation("Generated {Count} slots for service {ServiceId} on {Date}",
            result.Value.Count(), serviceId, date.Date);
        return Ok(result.Value);
    }

    /// <summary>Delete time slot by ID</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken ct)
    {
        if (id == Guid.Empty)
            return BadRequest(new { Error = "Invalid time slot ID." });

        var result = await _svc.DeleteAsync(id, ct);
        if (!result.IsSuccess)
        {
            if (result.ErrorMessage.Contains("not found"))
                return NotFound(new { result.ErrorMessage });

            _logger.LogError("DeleteAsync failed for slot {Id}: {Error}", id, result.ErrorMessage);
            return StatusCode(500, new { result.ErrorMessage });
        }

        _logger.LogInformation("Deleted time slot {Id}", id);
        return NoContent();
    }
}