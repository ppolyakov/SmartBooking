using Microsoft.EntityFrameworkCore;
using SmartBooking.Domain.Entities;
using SmartBooking.Infrastructure.Persistence;
using SmartBooking.Shared;
using SmartBooking.Shared.Dto;
using SmartBooking.WebAPI.Services.Interfaces;

public class TimeSlotsService : ITimeSlotsService
{
    private readonly AppDbContext _db;
    private readonly ILogger<TimeSlotsService> _logger;

    public TimeSlotsService(AppDbContext db, ILogger<TimeSlotsService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<TimeSlotDto>>> GetAllAsync(CancellationToken ct = default)
    {
        try
        {
            var slots = await _db.TimeSlots
                .AsNoTracking()
                .Include(ts => ts.Service)
                .Include(ts => ts.Booking)
                .ToListAsync(ct);

            var dtos = slots.Select(ts => new TimeSlotDto
            {
                Id = ts.Id,
                StartTime = ts.StartTime,
                ServiceTitle = ts.Service.Title,
                IsBooked = ts.Booking != null
            }).ToList();

            return Result<IEnumerable<TimeSlotDto>>.Success(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetAllAsync");
            return Result<IEnumerable<TimeSlotDto>>.Failure("Could not load time slots.");
        }
    }

    public async Task<Result<IEnumerable<TimeSlotDto>>> GenerateAsync(
        Guid serviceId, DateTime date, CancellationToken ct = default)
    {
        try
        {
            var service = await _db.Services.FindAsync(new object[] { serviceId }, ct);
            if (service == null)
            {
                _logger.LogWarning("Service {ServiceId} not found.", serviceId);
                return Result<IEnumerable<TimeSlotDto>>.Failure("Service not found.");
            }

            var slotLength = TimeSpan.FromMinutes(service.Duration);
            var start = date.Date.AddHours(9);
            var end = date.Date.AddHours(17);

            var created = new List<TimeSlot>();
            while (start.Add(slotLength) <= end)
            {
                var exists = await _db.TimeSlots.AnyAsync(t =>
                    t.ServiceId == serviceId && t.StartTime == start, ct);

                if (!exists)
                {
                    created.Add(new TimeSlot
                    {
                        ServiceId = serviceId,
                        StartTime = start
                    });
                }

                start = start.Add(slotLength);
            }

            if (created.Any())
            {
                _db.TimeSlots.AddRange(created);
                await _db.SaveChangesAsync(ct);
            }

            var dtos = created.Select(ts => new TimeSlotDto
            {
                Id = ts.Id,
                StartTime = ts.StartTime,
                ServiceTitle = service.Title,
                IsBooked = false
            }).ToList();

            return Result<IEnumerable<TimeSlotDto>>.Success(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GenerateAsync");
            return Result<IEnumerable<TimeSlotDto>>.Failure("Could not generate time slots.");
        }
    }

    public async Task<Result<bool>> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        try
        {
            var slot = await _db.TimeSlots.FindAsync(new object[] { id }, ct);
            if (slot == null)
            {
                _logger.LogWarning("TimeSlot {Id} not found.", id);
                return Result<bool>.Failure("Time slot not found.");
            }

            _db.TimeSlots.Remove(slot);
            await _db.SaveChangesAsync(ct);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in DeleteAsync");
            return Result<bool>.Failure("Could not delete time slot.");
        }
    }
}