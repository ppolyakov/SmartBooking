using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartBooking.Domain.Entities;
using SmartBooking.Infrastructure.Persistence;

namespace SmartBooking.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly AppDbContext _db;

    public BookingsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Booking>>> GetAll()
    {
        return await _db.Bookings
            .Include(b => b.Client)
            .Include(b => b.TimeSlot)
            .ThenInclude(t => t.Service)
            .ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult> Create(Guid clientId, Guid timeSlotId)
    {
        var client = await _db.Clients.FindAsync(clientId);
        var slot = await _db.TimeSlots
            .Include(s => s.Booking)
            .FirstOrDefaultAsync(s => s.Id == timeSlotId);

        if (client == null || slot == null)
            return NotFound("Client or TimeSlot not found.");

        if (slot.Booking != null)
            return BadRequest("TimeSlot already booked.");

        var booking = new Booking
        {
            ClientId = clientId,
            TimeSlotId = timeSlotId
        };

        _db.Bookings.Add(booking);
        await _db.SaveChangesAsync();

        return Ok(booking);
    }
}