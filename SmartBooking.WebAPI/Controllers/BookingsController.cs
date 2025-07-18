using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartBooking.Domain.Entities;
using SmartBooking.Infrastructure.Identity;
using SmartBooking.Infrastructure.Persistence;
using SmartBooking.Shared.Http.Requests;
using SmartBooking.WebAPI.Models;

namespace SmartBooking.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public BookingsController(AppDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Booking>>> GetAll()
    {
        var bookings = await _db.Bookings
                .Include(b => b.TimeSlot)
                .ThenInclude(ts => ts.Service)
                .ToListAsync();

        var result = new List<BookingDto>();
        foreach (var b in bookings)
        {
            var user = await _userManager.FindByIdAsync(b.UserId.ToString());
            result.Add(new BookingDto
            {
                Id = b.Id,
                StartTime = b.TimeSlot.StartTime,
                ServiceTitle = b.TimeSlot.Service.Title,
                ClientName = user?.UserName ?? "(unknown)",
                ClientEmail = user?.Email ?? "(unknown)"
            });
        }
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] BookSlotRequest request)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null)
            return NotFound("User not found.");

        var slot = await _db.TimeSlots
            .Include(s => s.Booking)
            .FirstOrDefaultAsync(s => s.Id == request.SlotId);

        if (slot == null)
            return NotFound("TimeSlot not found.");

        if (slot.Booking != null)
            return BadRequest("TimeSlot already booked.");

        var booking = new Booking
        {
            UserId = request.UserId,
            TimeSlotId = request.SlotId
        };
        _db.Bookings.Add(booking);
        await _db.SaveChangesAsync();

        return Ok(booking);
    }

    [HttpGet("all")]
    public async Task<ActionResult<List<BookingDto>>> GetAllBookings()
    {
        var bookings = await _db.Bookings
        .Include(b => b.TimeSlot)
            .ThenInclude(ts => ts.Service)
        .ToListAsync();

        var result = new List<BookingDto>(bookings.Count);
        foreach (var b in bookings)
        {
            var user = await _userManager.FindByIdAsync(b.UserId.ToString());

            result.Add(new BookingDto
            {
                Id = b.Id,
                StartTime = b.TimeSlot.StartTime,
                ServiceTitle = b.TimeSlot.Service.Title,
                ClientName = user?.UserName ?? "(unknown)",
                ClientEmail = user?.Email ?? "(unknown)"
            });
        }

        return Ok(result);
    }
}