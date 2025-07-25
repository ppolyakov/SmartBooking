using Microsoft.AspNetCore.Mvc;
using SmartBooking.Shared.Dto;
using SmartBooking.Shared.Http.Requests;
using SmartBooking.WebAPI.Models;
using SmartBooking.WebAPI.Services.Interfaces;

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;
    private readonly ILogger<BookingsController> _logger;

    public BookingsController(IBookingService bookingService, ILogger<BookingsController> logger)
    {
        _bookingService = bookingService;
        _logger = logger;
    }

    /// <summary>
    /// Retrieve all bookings (Admin only).
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<BookingDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<BookingDto>>> GetAllAsync(CancellationToken ct)
    {
        var result = await _bookingService.GetAllBookingsAsync(ct);
        if (!result.IsSuccess)
        {
            _logger.LogError("Failed to retrieve bookings: {Error}", result.ErrorMessage);
            return StatusCode(StatusCodes.Status500InternalServerError, new { result.ErrorMessage });
        }
        _logger.LogInformation("Retrieved {Count} bookings.", ((IEnumerable<BookingDto>)result.Value).Count());
        return Ok(result.Value);
    }

    /// <summary>
    /// Create a new booking for the current user.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(BookingDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BookingDto>> CreateAsync([FromBody] BookSlotRequest request, CancellationToken ct)
    {
        var result = await _bookingService.CreateBookingAsync(request, ct);
        if (!result.IsSuccess)
        {
            _logger.LogWarning("CreateBookingAsync failed for User {UserId}, Slot {SlotId}: {Error}", request.UserId, request.SlotId, result.ErrorMessage);

            return BadRequest(new { result.ErrorMessage });
        }

        // Service returns a BookingDto
        var dto = result.Value;

        _logger.LogInformation("Booking {BookingId} created.", dto.Id);
        // Use the named route "GetBookingById" so clients can locate the new resource
        return CreatedAtRoute(routeName: "GetBookingById", routeValues: new { id = dto.Id }, value: dto);
    }
}