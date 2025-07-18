using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using SmartBooking.BlazorUI.Models;
using SmartBooking.BlazorUI.Services.Interfaces;
using System.Net.Http.Headers;

namespace SmartBooking.BlazorUI.Services;

public class TimeSlotService : ITimeSlotsService
{
    private readonly IHttpClientFactory _factory;
    private readonly ProtectedLocalStorage _storage;

    private readonly HttpClient _http;

    public TimeSlotService(IHttpClientFactory factory, ProtectedLocalStorage storage)
    {
        _factory = factory;
        _storage = storage;
        _http = _factory.CreateClient("SmartBookingAPI");
    }

    public async Task<List<TimeSlotDto>> GetTimeSlotsAsync()
    {
        var tokenResult = await _storage.GetAsync<string>("authToken");
        if (tokenResult.Success && !string.IsNullOrEmpty(tokenResult.Value))
        {
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", tokenResult.Value);
        }

        return await _http.GetFromJsonAsync<List<TimeSlotDto>>("timeslots")
               ?? new List<TimeSlotDto>();
    }

    public async Task<bool> BookSlotAsync(Guid slotId, Guid clientId)
    {
        var tokenResult = await _storage.GetAsync<string>("authToken");
        if (tokenResult.Success && !string.IsNullOrEmpty(tokenResult.Value))
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", tokenResult.Value);

        var response = await _http.PostAsync($"bookings?clientId={clientId}&timeSlotId={slotId}", null);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteSlotAsync(Guid slotId)
    {
        var tokenResult = await _storage.GetAsync<string>("authToken");
        if (tokenResult.Success && !string.IsNullOrEmpty(tokenResult.Value))
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", tokenResult.Value);

        var response = await _http.DeleteAsync($"timeslots/{slotId}");
        return response.IsSuccessStatusCode;
    }
}