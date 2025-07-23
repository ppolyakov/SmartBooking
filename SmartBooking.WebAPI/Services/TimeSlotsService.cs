using Microsoft.EntityFrameworkCore;
using SmartBooking.Domain.Entities;
using SmartBooking.Infrastructure.Persistence;
using SmartBooking.Shared;
using SmartBooking.Shared.Dto;
using SmartBooking.WebAPI.Services.Interfaces;

namespace SmartBooking.WebAPI.Services;

public class TimeSlotsService(AppDbContext db, ILogger<TimeSlotsService> logger) : ITimeSlotsService
{
    public async Task<Result<bool>> Delete(Guid id)
    {
        var slot = await db.TimeSlots.FindAsync(id);
        if (slot is null)
        {
            logger.LogWarning("TimeSlot with ID {Id} not found.", id);
            return Result<bool>.Failure($"TimeSlot with ID {id} not found.");
        }

        db.TimeSlots.Remove(slot);
        await db.SaveChangesAsync();

        logger.LogInformation("TimeSlot with ID {Id} deleted successfully.", id);
        return Result<bool>.Success(true);
    }

    public async Task<Result<IEnumerable<TimeSlot>>> GenerateSlots(Guid serviceId, DateTime date)
    {
        try
        {
            var service = await db.Services.FindAsync(serviceId);
            if (service == null)
            {
                logger.LogWarning("Service with ID {ServiceId} not found.", serviceId);
                return Result<IEnumerable<TimeSlot>>.Failure("Service not found.");
            }

            var slots = new List<TimeSlot>();
            var start = date.Date.AddHours(9); // Start 9:00
            var end = date.Date.AddHours(17);  // End 17:00

            while (start + service.Duration <= end)
            {
                if (!await db.TimeSlots.AnyAsync(t => t.StartTime == start && t.ServiceId == serviceId))
                {
                    slots.Add(new TimeSlot
                    {
                        StartTime = start,
                        ServiceId = serviceId
                    });
                }

                start = start.Add(service.Duration);
            }

            db.TimeSlots.AddRange(slots);
            await db.SaveChangesAsync();

            return Result<IEnumerable<TimeSlot>>.Success(slots);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error generating time slots.");
            return Result<IEnumerable<TimeSlot>>.Failure($"Error generating time slots: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<TimeSlotDto>>> GetAllTimeSlotsAsync()
    {
        try
        {
            var slots = await db.TimeSlots
                .Include(ts => ts.Service)
                .Include(ts => ts.Booking)
                .ToListAsync();

            var dtos = slots.Select(ts => new TimeSlotDto
            {
                Id = ts.Id,
                StartTime = ts.StartTime,
                ServiceTitle = ts.Service.Title,
                IsBooked = ts.Booking != null
            });
            return Result<IEnumerable<TimeSlotDto>>.Success(dtos);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving time slots.");
            return Result<IEnumerable<TimeSlotDto>>.Failure($"Error retrieving time slots: {ex.Message}");
        }
    }
}