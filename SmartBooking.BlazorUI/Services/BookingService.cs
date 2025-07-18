using SmartBooking.BlazorUI.Models;
using SmartBooking.BlazorUI.Services.Interfaces;

namespace SmartBooking.BlazorUI.Services;

public class BookingService(HttpClient httpClient) : IBookingService
{
    public async Task<List<BookingDto>> GetAllAsync()
    {
        return await httpClient.GetFromJsonAsync<List<BookingDto>>("bookings/all") ?? [];
    }
}