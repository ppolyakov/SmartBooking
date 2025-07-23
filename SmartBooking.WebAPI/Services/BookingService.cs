using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartBooking.Domain.Entities;
using SmartBooking.Infrastructure.Identity;
using SmartBooking.Infrastructure.Persistence;
using SmartBooking.Shared;
using SmartBooking.Shared.Dto;
using SmartBooking.Shared.Http.Requests;
using SmartBooking.WebAPI.Services.Interfaces;

namespace SmartBooking.WebAPI.Services;

public class BookingService(AppDbContext db, UserManager<ApplicationUser> userManager, ILogger<BookingService> logger) : IBookingService
{
    public async Task<Result<Booking>> CreateBookingAsync(BookSlotRequest request)
    {
        try
        {
            var user = await userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
            {
                logger.LogWarning("User with ID {UserId} not found.", request.UserId);
                return Result<Booking>.Failure("User not found.");
            }

            var slot = await db.TimeSlots
                .Include(s => s.Booking)
                .FirstOrDefaultAsync(s => s.Id == request.SlotId);

            if (slot == null)
            {
                logger.LogWarning("TimeSlot with ID {SlotId} not found.", request.SlotId);
                return Result<Booking>.Failure("TimeSlot not found.");
            }

            if (slot.Booking != null)
            {
                logger.LogWarning("TimeSlot with ID {SlotId} is already booked.", request.SlotId);
                return Result<Booking>.Failure("TimeSlot is already booked.");
            }

            var booking = new Booking
            {
                UserId = request.UserId,
                TimeSlotId = request.SlotId
            };
            db.Bookings.Add(booking);
            await db.SaveChangesAsync();

            return Result<Booking>.Success(booking);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating booking.");
            return Result<Booking>.Failure($"Error creating booking: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<BookingDto>>> GetAllBookingsAsync()
    {
        try
        {
            var bookings = await db.Bookings
                    .Include(b => b.TimeSlot)
                    .ThenInclude(ts => ts.Service)
                    .ToListAsync();

            var result = new List<BookingDto>();
            foreach (var b in bookings)
            {
                var user = await userManager.FindByIdAsync(b.UserId.ToString());
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
                logger.LogInformation("No bookings found.");
                return Result<IEnumerable<BookingDto>>.Success(new List<BookingDto>());
            }

            logger.LogInformation("Retrieved {Count} bookings.", result.Count);
            return Result<IEnumerable<BookingDto>>.Success(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving bookings.");
            return Result<IEnumerable<BookingDto>>.Failure($"Error retrieving bookings: {ex.Message}");
        }
    }
}