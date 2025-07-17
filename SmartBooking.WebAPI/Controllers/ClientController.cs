using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartBooking.Domain.Entities;
using SmartBooking.Infrastructure.Persistence;

namespace SmartBooking.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientsController : ControllerBase
{
    private readonly AppDbContext _db;

    public ClientsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Client>>> GetAll()
    {
        return await _db.Clients.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Client>> GetById(Guid id)
    {
        var client = await _db.Clients.FindAsync(id);
        return client is null ? NotFound() : Ok(client);
    }

    [HttpPost]
    public async Task<ActionResult<Client>> Create(Client client)
    {
        _db.Clients.Add(client);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = client.Id }, client);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, Client updated)
    {
        var client = await _db.Clients.FindAsync(id);
        if (client is null) return NotFound();

        client.Name = updated.Name;
        client.Email = updated.Email;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var client = await _db.Clients.FindAsync(id);
        if (client is null) return NotFound();

        _db.Clients.Remove(client);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}