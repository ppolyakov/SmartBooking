using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartBooking.Domain.Entities;
using SmartBooking.Infrastructure.Identity;
using SmartBooking.Infrastructure.Persistence;
using SmartBooking.Shared;
using SmartBooking.Shared.Dto;
using SmartBooking.WebAPI.Services.Interfaces;

namespace SmartBooking.WebAPI.Services;

public class ServiceService(AppDbContext db, UserManager<ApplicationUser> userManager, ILogger<ServiceService> logger) : IServiceService
{
    public async Task<Result<Service>> CreateServiceAsync(Service service)
    {
        try
        {
            db.Services.Add(service);
            await db.SaveChangesAsync();
            return Result<Service>.Success(service);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating service.");
            return Result<Service>.Failure($"Error creating service: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<Service>>> GetAllServicesAsync()
    {
        try
        {
            var services = await db.Services
                    .Include(s => s.TimeSlots)
                    .ToListAsync();
            return Result<IEnumerable<Service>>.Success(services);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving services.");
            return Result<IEnumerable<Service>>.Failure($"Error retrieving services: {ex.Message}");
        }
    }

    public async Task<Result<List<ServiceWithSlotsDto>>> GetAllServicesWithSlotsAsync()
    {
        try
        {
            var services = await db.Services
                    .Include(s => s.TimeSlots)
                        .ThenInclude(ts => ts.Booking)
                    .ToListAsync();

            var result = new List<ServiceWithSlotsDto>(services.Count);

            foreach (var service in services)
            {
                var slots = new List<TimeSlotWithClientDto>(service.TimeSlots.Count);

                foreach (var ts in service.TimeSlots.OrderBy(ts => ts.StartTime))
                {
                    string? email = null;
                    if (ts.Booking != null)
                    {
                        var user = await userManager.FindByIdAsync(ts.Booking.UserId.ToString());
                        email = user?.Email;
                    }

                    slots.Add(new TimeSlotWithClientDto
                    {
                        Id = ts.Id,
                        StartTime = ts.StartTime,
                        IsBooked = ts.Booking != null,
                        ClientEmail = email
                    });
                }

                result.Add(new ServiceWithSlotsDto
                {
                    Id = service.Id,
                    Title = service.Title,
                    Duration = service.Duration,
                    Slots = slots
                });
            }

            return Result<List<ServiceWithSlotsDto>>.Success(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving services with slots.");
            return Result<List<ServiceWithSlotsDto>>.Failure($"Error retrieving services with slots: {ex.Message}");
        }
    }
}