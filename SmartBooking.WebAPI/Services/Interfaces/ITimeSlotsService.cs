using SmartBooking.Domain.Entities;
using SmartBooking.Shared;
using SmartBooking.Shared.Dto;

namespace SmartBooking.WebAPI.Services.Interfaces;

public interface ITimeSlotsService
{
    Task<Result<IEnumerable<TimeSlotDto>>> GetAllAsync(CancellationToken ct = default);
    Task<Result<IEnumerable<TimeSlotDto>>> GenerateAsync(Guid serviceId, DateTime date, CancellationToken ct = default);
    Task<Result<bool>> DeleteAsync(Guid id, CancellationToken ct = default);
}