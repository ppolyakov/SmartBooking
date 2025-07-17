using SmartBooking.BlazorUI.Models;

namespace SmartBooking.BlazorUI.Services;

public interface ITimeSlotsService
{
    Task<List<TimeSlotDto>> GetTimeSlotsAsync();
    Task<bool> BookSlotAsync(Guid slotId, Guid clientId);
}