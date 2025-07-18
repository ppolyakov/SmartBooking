using SmartBooking.BlazorUI.Helpers;
using SmartBooking.BlazorUI.Services.Interfaces;
using SmartBooking.Shared.Dto;

namespace SmartBooking.BlazorUI.Services;

public class BookingService(HttpClient httpClient, ILogger<BookingService> logger) : IBookingService
{
    public async Task<Result<List<BookingDto>>> GetAllAsync()
    {
        try
        {
            var bookings = await httpClient.GetFromJsonAsync<List<BookingDto>>("bookings");
            if (bookings == null)
            {
                logger.LogWarning("No bookings found.");
                return Result<List<BookingDto>>.Failure("No bookings found");
            }
            return Result<List<BookingDto>>.Success(bookings ?? new List<BookingDto>());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error fetching bookings.");
            return Result<List<BookingDto>>.Failure("Unexpected error");
        }
    }
}