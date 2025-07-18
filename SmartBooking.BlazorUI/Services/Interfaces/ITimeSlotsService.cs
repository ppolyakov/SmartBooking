using SmartBooking.BlazorUI.Helpers;
using SmartBooking.Shared.Dto;
using SmartBooking.Shared.Http.Requests;

namespace SmartBooking.BlazorUI.Services.Interfaces;

public interface ITimeSlotsService
{
    Task<Result<List<TimeSlotDto>>> GetTimeSlotsAsync();
    Task<Result<bool>> BookSlotAsync(BookSlotRequest request);
    Task<Result<bool>> DeleteSlotAsync(Guid slotId);
}