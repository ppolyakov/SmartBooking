using SmartBooking.BlazorUI.Models;

namespace SmartBooking.BlazorUI.Services;

public class TimeSlotService : ITimeSlotsService
{
    private readonly HttpClient _http;

    public TimeSlotService(IHttpClientFactory factory)
    {
        _http = factory.CreateClient("SmartBookingAPI");
    }

    public async Task<List<TimeSlotDto>> GetTimeSlotsAsync()
    {
        return await _http.GetFromJsonAsync<List<TimeSlotDto>>("timeslots") ?? [];
    }

    public async Task<bool> BookSlotAsync(Guid slotId, Guid clientId)
    {
        var response = await _http.PostAsync($"bookings?clientId={clientId}&timeSlotId={slotId}", null);
        return response.IsSuccessStatusCode;
    }
}