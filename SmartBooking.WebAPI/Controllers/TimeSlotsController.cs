using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartBooking.Domain.Entities;
using SmartBooking.Infrastructure.Persistence;
using SmartBooking.WebAPI.Models;

namespace SmartBooking.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TimeSlotsController : ControllerBase
{
    private readonly AppDbContext _db;

    public TimeSlotsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TimeSlotDto>>> GetAll()
    {
        var slots = await _db.TimeSlots
            .Include(ts => ts.Service)
            .Include(ts => ts.Booking)
            .ToListAsync();

        var dtos = slots.Select(ts => new TimeSlotDto
        {
            Id = ts.Id,
            StartTime = ts.StartTime,
            ServiceTitle = ts.Service.Title,
            IsBooked = ts.Booking != null
        });

        return Ok(dtos);
    }

    [HttpPost("generate")]
    public async Task<IActionResult> GenerateSlots([FromQuery] Guid serviceId, [FromQuery] DateTime date)
    {
        var service = await _db.Services.FindAsync(serviceId);
        if (service == null)
            return NotFound("Service not found.");

        var slots = new List<TimeSlot>();
        var start = date.Date.AddHours(9); // Start 9:00
        var end = date.Date.AddHours(17);  // End 17:00

        while (start + service.Duration <= end)
        {
            if (!await _db.TimeSlots.AnyAsync(t => t.StartTime == start && t.ServiceId == serviceId))
            {
                slots.Add(new TimeSlot
                {
                    StartTime = start,
                    ServiceId = serviceId
                });
            }

            start = start.Add(service.Duration);
        }

        _db.TimeSlots.AddRange(slots);
        await _db.SaveChangesAsync();

        return Ok(slots);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var slot = await _db.TimeSlots.FindAsync(id);
        if (slot is null) return NotFound();

        _db.TimeSlots.Remove(slot);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}