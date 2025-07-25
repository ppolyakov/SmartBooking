using SmartBooking.Domain.Entities;
using SmartBooking.Shared;
using SmartBooking.Shared.Dto;

namespace SmartBooking.WebAPI.Services.Interfaces;

public interface IServiceService
{
    Task<Result<IEnumerable<ServiceDto>>> GetAllAsync(CancellationToken ct = default);
    Task<Result<ServiceDto>> CreateAsync(CreateServiceDto dto, CancellationToken ct = default);
    Task<Result<IEnumerable<ServiceWithSlotsDto>>> GetWithSlotsAsync(CancellationToken ct = default);
}