using Microsoft.AspNetCore.Mvc;
using SmartBooking.Domain.Entities;
using SmartBooking.WebAPI.Models;
using SmartBooking.WebAPI.Services.Interfaces;

namespace SmartBooking.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServicesController(IServiceService serviceService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Service>>> GetAll()
    {
        var result = await serviceService.GetAllServicesAsync();
        if (!result.IsSuccess)
            return BadRequest(result.ErrorMessage);

        var services = result.Value;
        return Ok(services);
    }

    [HttpPost]
    public async Task<ActionResult<Service>> Create(Service service)
    {
        if (service == null)
        {
            return BadRequest(new { Error = "Invalid service data." });
        }
        var result = await serviceService.CreateServiceAsync(service);
        if (!result.IsSuccess)
        {
            return BadRequest(new { Error = result.ErrorMessage });
        }
        service = result.Value;
        return Ok(service);
    }

    [HttpGet("full")]
    public async Task<ActionResult<List<ServiceWithSlotsDto>>> GetAllWithSlots()
    {
        var result = await serviceService.GetAllServicesWithSlotsAsync();
        if (!result.IsSuccess)
            return BadRequest(result.ErrorMessage);
        var servicesWithSlots = result.Value;
        return Ok(servicesWithSlots);
    }
}