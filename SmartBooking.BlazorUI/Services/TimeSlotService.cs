using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using SmartBooking.BlazorUI.Models;
using SmartBooking.BlazorUI.Services.Interfaces;
using System.Net.Http.Headers;

namespace SmartBooking.BlazorUI.Services;

public class TimeSlotService(HttpClient httpClient, ProtectedLocalStorage storage) : ITimeSlotsService
{
    public async Task<List<TimeSlotDto>> GetTimeSlotsAsync()
    {
        var tokenResult = await storage.GetAsync<string>("authToken");
        if (tokenResult.Success && !string.IsNullOrEmpty(tokenResult.Value))
        {
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", tokenResult.Value);
        }

        return await httpClient.GetFromJsonAsync<List<TimeSlotDto>>("timeslots")
               ?? new List<TimeSlotDto>();
    }

    public async Task<bool> BookSlotAsync(Guid slotId, Guid clientId)
    {
        var tokenResult = await storage.GetAsync<string>("authToken");
        if (tokenResult.Success && !string.IsNullOrEmpty(tokenResult.Value))
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", tokenResult.Value);

        var response = await httpClient.PostAsync($"bookings?clientId={clientId}&timeSlotId={slotId}", null);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteSlotAsync(Guid slotId)
    {
        var tokenResult = await storage.GetAsync<string>("authToken");
        if (tokenResult.Success && !string.IsNullOrEmpty(tokenResult.Value))
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", tokenResult.Value);

        var response = await httpClient.DeleteAsync($"timeslots/{slotId}");
        return response.IsSuccessStatusCode;
    }
}