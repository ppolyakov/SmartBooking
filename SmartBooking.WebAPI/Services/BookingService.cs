using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartBooking.Domain.Entities;
using SmartBooking.Infrastructure.Persistence;
using SmartBooking.Shared;
using SmartBooking.Shared.Dto;
using SmartBooking.Shared.Http.Requests;
using SmartBooking.WebAPI.Services.Interfaces;

public class BookingService(AppDbContext db, UserManager<ApplicationUser> userManager, ILogger<BookingService> logger) : IBookingService
{
    private readonly AppDbContext _db = db;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly ILogger<BookingService> _logger = logger;

    public async Task<Result<BookingDto>> CreateBookingAsync(BookSlotRequest request, CancellationToken ct = default)
    {
        // 1) Validate user
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null)
            return Result<BookingDto>.Failure("User not found.");

        // 2) Validate slot
        var slot = await _db.TimeSlots
            .Include(ts => ts.Booking)
            .Include(ts => ts.Service)
            .FirstOrDefaultAsync(ts => ts.Id == request.SlotId, ct);

        if (slot == null)
            return Result<BookingDto>.Failure("Time slot not found.");

        if (slot.Booking != null)
            return Result<BookingDto>.Failure("Time slot already booked.");

        // 3) Create booking
        var booking = new Booking
        {
            UserId = request.UserId,
            TimeSlotId = slot.Id
        };
        _db.Bookings.Add(booking);
        await _db.SaveChangesAsync(ct);

        _logger.LogInformation("User {UserId} booked slot {SlotId} (BookingId={BookingId}).",
            user.Id, slot.Id, booking.Id);

        // 4) Map to DTO
        var dto = new BookingDto
        {
            Id = booking.Id,
            StartTime = slot.StartTime,
            ServiceTitle = slot.Service.Title,
            ClientName = user.UserName!,
            ClientEmail = user.Email!
        };

        return Result<BookingDto>.Success(dto);
    }

    public async Task<Result<IEnumerable<BookingDto>>> GetAllBookingsAsync(CancellationToken ct = default)
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

        if (result.Count == 0)
        {
            _logger.LogInformation("No bookings found.");
            return Result<IEnumerable<BookingDto>>.Success(new List<BookingDto>());
        }

        _logger.LogInformation("Retrieved {Count} bookings.", result.Count);
        return Result<IEnumerable<BookingDto>>.Success(result);
    }
}