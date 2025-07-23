using Microsoft.AspNetCore.Mvc;
using SmartBooking.Domain.Entities;
using SmartBooking.Shared.Http.Requests;
using SmartBooking.WebAPI.Models;
using SmartBooking.WebAPI.Services.Interfaces;

namespace SmartBooking.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingsController(IBookingService bookingService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Booking>>> GetAll()
    {
        var result = await bookingService.GetAllBookingsAsync();
        if (!result.IsSuccess)
            return BadRequest(result.ErrorMessage);
        var bookings = result.Value;

        return Ok(bookings);
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] BookSlotRequest request)
    {
        if (request == null)
        {
            return BadRequest(new { Error = "Invalid booking request." });
        }
        var result = await bookingService.CreateBookingAsync(request);
        if (!result.IsSuccess)
        {
            return BadRequest(new { Error = result.ErrorMessage });
        }
        var booking = result.Value;
        return Ok(booking);
    }

    [HttpGet("all")]
    public async Task<ActionResult<List<BookingDto>>> GetAllBookings()
    {
        var result = await bookingService.GetAllBookingsAsync();
        if (!result.IsSuccess)
            return BadRequest(result.ErrorMessage);

        var bookings = result.Value;
        return Ok(bookings);
    }
}