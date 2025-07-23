using SmartBooking.Domain.Entities;
using SmartBooking.Shared;
using SmartBooking.Shared.Dto;

namespace SmartBooking.WebAPI.Services.Interfaces;

public interface ITimeSlotsService
{
    Task<Result<IEnumerable<TimeSlotDto>>> GetAllTimeSlotsAsync();
    Task<Result<IEnumerable<TimeSlot>>> GenerateSlots(Guid serviceId, DateTime date);
    Task<Result<bool>> Delete(Guid id);
}