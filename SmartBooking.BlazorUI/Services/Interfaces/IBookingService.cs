using SmartBooking.BlazorUI.Models;

namespace SmartBooking.BlazorUI.Services.Interfaces;

public interface IBookingService
{
    Task<List<BookingDto>> GetAllAsync();
}