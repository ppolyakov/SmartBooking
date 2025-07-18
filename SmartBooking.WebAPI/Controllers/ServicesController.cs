using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartBooking.Domain.Entities;
using SmartBooking.Infrastructure.Identity;
using SmartBooking.Infrastructure.Persistence;
using SmartBooking.WebAPI.Models;

namespace SmartBooking.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServicesController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public ServicesController(AppDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Service>>> GetAll()
    {
        var services = await _db.Services
                .Include(s => s.TimeSlots)
                .ToListAsync();

        return Ok(services);
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
                .ToListAsync();

        var result = new List<ServiceWithSlotsDto>(services.Count);

        foreach (var service in services)
        {
            var slots = new List<TimeSlotWithClientDto>(service.TimeSlots.Count);

            foreach (var ts in service.TimeSlots.OrderBy(ts => ts.StartTime))
            {
                string? email = null;
                if (ts.Booking != null)
                {
                    var user = await _userManager.FindByIdAsync(ts.Booking.UserId.ToString());
                    email = user?.Email;
                }

                slots.Add(new TimeSlotWithClientDto
                {
                    Id = ts.Id,
                    StartTime = ts.StartTime,
                    IsBooked = ts.Booking != null,
                    ClientEmail = email
                });
            }

            result.Add(new ServiceWithSlotsDto
            {
                Id = service.Id,
                Title = service.Title,
                Duration = service.Duration,
                Slots = slots
            });
        }

        return Ok(result);
    }
}