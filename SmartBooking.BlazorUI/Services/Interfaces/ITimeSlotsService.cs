using SmartBooking.BlazorUI.Models;

namespace SmartBooking.BlazorUI.Services.Interfaces;

public interface ITimeSlotsService
{
    Task<List<TimeSlotDto>> GetTimeSlotsAsync();
    Task<bool> BookSlotAsync(Guid slotId, Guid clientId);
    Task<bool> DeleteSlotAsync(Guid slotId);
}