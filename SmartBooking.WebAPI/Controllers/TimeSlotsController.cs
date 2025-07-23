using Microsoft.AspNetCore.Mvc;
using SmartBooking.WebAPI.Models;
using SmartBooking.WebAPI.Services.Interfaces;

namespace SmartBooking.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TimeSlotsController(ITimeSlotsService timeSlotsService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TimeSlotDto>>> GetAll()
    {
        var result = await timeSlotsService.GetAllTimeSlotsAsync();
        if (!result.IsSuccess)
            return BadRequest(result.ErrorMessage);

        var timeSlots = result.Value;
        return Ok(timeSlots);
    }

    [HttpPost("generate")]
    public async Task<IActionResult> GenerateSlots([FromQuery] Guid serviceId, [FromQuery] DateTime date)
    {
        if (serviceId == Guid.Empty || date == default)
        {
            return BadRequest(new { Error = "Invalid service ID or date." });
        }
        var result = await timeSlotsService.GenerateSlots(serviceId, date);
        if (!result.IsSuccess)
        {
            return BadRequest(new { Error = result.ErrorMessage });
        }
        var slots = result.Value;
        return Ok(slots);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        if (id == Guid.Empty)
        {
            return BadRequest(new { Error = "Invalid time slot ID." });
        }
        var result = await timeSlotsService.Delete(id);
        if (!result.IsSuccess)
        {
            return BadRequest(new { Error = result.ErrorMessage });
        }

        return NoContent();
    }
}