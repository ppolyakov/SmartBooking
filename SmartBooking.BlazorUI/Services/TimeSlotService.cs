using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using SmartBooking.BlazorUI.Helpers;
using SmartBooking.BlazorUI.Services.Interfaces;
using SmartBooking.Shared.Dto;
using SmartBooking.Shared.Http.Requests;
using System.Net.Http.Headers;
using System.Text.Json;

namespace SmartBooking.BlazorUI.Services;

public class TimeSlotService(HttpClient httpClient, ProtectedLocalStorage storage, ILogger<TimeSlotService> logger) : ITimeSlotsService
{
    public async Task<Result<List<TimeSlotDto>>> GetTimeSlotsAsync()
    {
        try 
        {
            var tokenResult = await storage.GetAsync<string>("authToken");
            if (tokenResult.Success && !string.IsNullOrEmpty(tokenResult.Value))
            {
                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", tokenResult.Value);
            }

            var timeSlots = await httpClient.GetFromJsonAsync<List<TimeSlotDto>>("timeslots");

            if (timeSlots == null)
            {
                logger.LogWarning("No time slots found.");
                return Result<List<TimeSlotDto>>.Failure("No time slots found");
            }

            return Result<List<TimeSlotDto>>.Success(timeSlots ?? new List<TimeSlotDto>());
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Error retrieving time slots.");
            return Result<List<TimeSlotDto>>.Failure("Error retrieving time slots");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unexpected error retrieving time slots.");
            return Result<List<TimeSlotDto>>.Failure("Unexpected error retrieving time slots");
        }
    }

    public async Task<Result<bool>> BookSlotAsync(BookSlotRequest request)
    {
        try 
        {
            var tokenResult = await storage.GetAsync<string>("authToken");
            if (tokenResult.Success && !string.IsNullOrEmpty(tokenResult.Value))
                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", tokenResult.Value);

            var response = await httpClient.PostAsJsonAsync("bookings", request);
            response.EnsureSuccessStatusCode();
            logger.LogInformation("Slot {slotId} booked successfully.", request.SlotId);
            return Result<bool>.Success(true);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Error booking slot.");
            return Result<bool>.Failure("Error booking slot");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unexpected error booking slot.");
            return Result<bool>.Failure("Unexpected error booking slot");
        }
    }

    public async Task<Result<bool>> DeleteSlotAsync(Guid slotId)
    {
        try 
        {
            var tokenResult = await storage.GetAsync<string>("authToken");
            if (tokenResult.Success && !string.IsNullOrEmpty(tokenResult.Value))
                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", tokenResult.Value);

            var response = await httpClient.DeleteAsync($"timeslots/{slotId}");
            response.EnsureSuccessStatusCode();
            logger.LogInformation("Slot {slotId} deleted successfully.", slotId);
            return Result<bool>.Success(true);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Error deleting slot {slotId}.", slotId);
            return Result<bool>.Failure("Error deleting slot");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unexpected error deleting slot {slotId}.", slotId);
            return Result<bool>.Failure("Unexpected error deleting slot");
        }
    }
}