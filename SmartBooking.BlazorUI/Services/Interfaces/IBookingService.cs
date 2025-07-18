using SmartBooking.BlazorUI.Helpers;
using SmartBooking.Shared.Dto;

namespace SmartBooking.BlazorUI.Services.Interfaces;

public interface IBookingService
{
    Task<Result<List<BookingDto>>> GetAllAsync();
}