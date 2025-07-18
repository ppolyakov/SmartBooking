using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartBooking.Domain.Entities;
using SmartBooking.Infrastructure.Persistence;
using SmartBooking.WebAPI.Models;

namespace SmartBooking.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServicesController : ControllerBase
{
    private readonly AppDbContext _db;

    public ServicesController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Service>>> GetAll()
    {
        return await _db.Services.Include(s => s.TimeSlots).ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<Service>> Create(Service service)
    {
        _db.Services.Add(service);
        await _db.SaveChangesAsync();
        return Ok(service);
    }

    [HttpGet("full")]
    public async Task<ActionResult<List<ServiceWithSlotsDto>>> GetAllWithSlots()
    {
        var services = await _db.Services
            .Include(s => s.TimeSlots)
            .ThenInclude(ts => ts.Booking)
            .ThenInclude(b => b.Client)
            .ToListAsync();

        var result = services.Select(service => new ServiceWithSlotsDto
        {
            Id = service.Id,
            Title = service.Title,
            Duration = service.Duration,
            Slots = service.TimeSlots
                .OrderBy(ts => ts.StartTime)
                .Select(ts => new TimeSlotWithClientDto
                {
                    Id = ts.Id,
                    StartTime = ts.StartTime,
                    IsBooked = ts.Booking != null,
                    ClientEmail = ts.Booking?.Client?.Email
                }).ToList()
        }).ToList();

        return Ok(result);
    }
}