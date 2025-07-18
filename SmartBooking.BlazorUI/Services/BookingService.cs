using SmartBooking.BlazorUI.Models;
using SmartBooking.BlazorUI.Services.Interfaces;

namespace SmartBooking.BlazorUI.Services;

public class BookingService : IBookingService
{
    private readonly HttpClient _http;

    public BookingService(IHttpClientFactory factory)
    {
        _http = factory.CreateClient("SmartBookingAPI");
    }

    public async Task<List<BookingDto>> GetAllAsync()
    {
        return await _http.GetFromJsonAsync<List<BookingDto>>("bookings/all") ?? [];
    }
}