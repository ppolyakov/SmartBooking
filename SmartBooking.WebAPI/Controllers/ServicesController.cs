using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartBooking.Domain.Entities;
using SmartBooking.Infrastructure.Persistence;

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
}