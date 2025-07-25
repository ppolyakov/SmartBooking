using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartBooking.Domain.Entities;
using SmartBooking.Infrastructure.Persistence;
using SmartBooking.Shared;
using SmartBooking.Shared.Dto;
using SmartBooking.WebAPI.Services.Interfaces;

public class ServiceService(AppDbContext db, UserManager<ApplicationUser> userManager, ILogger<ServiceService> logger) : IServiceService
{
    private readonly AppDbContext _db = db;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly ILogger<ServiceService> _logger = logger;

    public async Task<Result<IEnumerable<ServiceDto>>> GetAllAsync(CancellationToken ct = default)
    {
        try
        {
            var items = await _db.Services
                .AsNoTracking()
                .ToListAsync(ct);

            var dtos = items.Select(s => new ServiceDto
            {
                Id = s.Id,
                Title = s.Title,
                Duration = s.Duration,
                Date = s.Date,
                Description = s.Description,
                Price = s.Price
            });

            _logger.LogInformation("Retrieved {Count} services.", dtos.Count());
            return Result<IEnumerable<ServiceDto>>.Success(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetAllAsync");
            return Result<IEnumerable<ServiceDto>>.Failure("Could not load services.");
        }
    }

    public async Task<Result<ServiceDto>> CreateAsync(CreateServiceDto dto, CancellationToken ct = default)
    {
        try
        {
            var entity = new Service
            {
                Title = dto.Title,
                Duration = dto.Duration,
                Date = dto.Date,
                Description = dto.Description,
                Price = dto.Price
            };

            _db.Services.Add(entity);
            await _db.SaveChangesAsync(ct);

            var result = new ServiceDto
            {
                Id = entity.Id,
                Title = entity.Title,
                Duration = entity.Duration,
                Date = entity.Date,
                Description = entity.Description,
                Price = entity.Price
            };

            _logger.LogInformation("Created Service {Id}.", entity.Id);
            return Result<ServiceDto>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in CreateAsync");
            return Result<ServiceDto>.Failure("Could not create service.");
        }
    }

    public async Task<Result<IEnumerable<ServiceWithSlotsDto>>> GetWithSlotsAsync(CancellationToken ct = default)
    {
        try
        {
            var services = await _db.Services
                .Include(s => s.TimeSlots)
                .ThenInclude(ts => ts.Booking)
                .AsNoTracking()
                .ToListAsync(ct);

            var result = new List<ServiceWithSlotsDto>(services.Count);

            foreach (var service in services)
            {
                var slots = new List<TimeSlotWithClientDto>(service.TimeSlots.Count);

                foreach (var ts in service.TimeSlots.OrderBy(ts => ts.StartTime))
                {
                    string? email = null;
                    if (ts.Booking is not null)
                    {
                        var user = await _userManager.FindByIdAsync(ts.Booking.UserId.ToString());
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
                    Date = service.Date,
                    Description = service.Description,
                    Price = service.Price,
                    Slots = slots
                });
            }

            _logger.LogInformation("Retrieved {Count} services with slots.", result.Count);
            return Result<IEnumerable<ServiceWithSlotsDto>>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving services with slots.");
            return Result<IEnumerable<ServiceWithSlotsDto>>.Failure("Error retrieving services with slots.");
        }
    }
}