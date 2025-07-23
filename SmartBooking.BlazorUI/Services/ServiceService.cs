using SmartBooking.BlazorUI.Helpers;
using SmartBooking.BlazorUI.Services.Interfaces;
using SmartBooking.Shared;
using SmartBooking.Shared.Dto;
using System.Text.Json;

namespace SmartBooking.BlazorUI.Services;

public class ServiceService(HttpClient httpClient, ILogger<ServiceService> logger) : IServiceService
{
    public async Task<Result<List<ServiceDto>>> GetAllServicesAsync()
    {
        try
        {
            var services = await httpClient.GetFromJsonAsync<List<ServiceDto>>("services");
            if (services == null)
            {
                logger.LogWarning("No services found.");
                return Result<List<ServiceDto>>.Failure("No services found");
            }

            return Result<List<ServiceDto>>.Success(services ?? new List<ServiceDto>());
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Error retrieving services.");
            return Result<List<ServiceDto>>.Failure("Error retrieving services");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unexpected error retrieving services.");
            return Result<List<ServiceDto>>.Failure("Unexpected error retrieving services");
        }
    }

    public async Task<Result<bool>> CreateServiceAsync(ServiceDto dto)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync("services", dto);
            response.EnsureSuccessStatusCode();
            return Result<bool>.Success(true);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Error creating service.");
            return Result<bool>.Failure("Error creating service");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unexpected error creating service.");
            return Result<bool>.Failure("Unexpected error creating service");
        }
    }

    public async Task<Result<bool>> GenerateSlotsAsync(Guid serviceId, DateTime date)
    {
        try
        {
            var url = $"timeslots/generate?serviceId={serviceId}&date={date:yyyy-MM-dd}";
            var response = await httpClient.PostAsync(url, null);
            response.EnsureSuccessStatusCode();
            return Result<bool>.Success(true);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Error generating slots for service.");
            return Result<bool>.Failure("Error generating slots");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unexpected error generating slots for service.");
            return Result<bool>.Failure("Unexpected error generating slots");
        }
    }

    public async Task<Result<List<ServiceWithSlotsDto>>> GetServicesWithSlotsAsync()
    {
        try
        {
            var servicesWithSlots = await httpClient.GetFromJsonAsync<List<ServiceWithSlotsDto>>("services/full");
            if (servicesWithSlots == null)
            {
                logger.LogWarning("No services with slots found.");
                return Result<List<ServiceWithSlotsDto>>.Failure("No services with slots found");
            }

            return Result<List<ServiceWithSlotsDto>>.Success(servicesWithSlots ?? new List<ServiceWithSlotsDto>());
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Error retrieving services with slots.");
            return Result<List<ServiceWithSlotsDto>>.Failure("Error retrieving services with slots");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unexpected error retrieving services with slots.");
            return Result<List<ServiceWithSlotsDto>>.Failure("Unexpected error retrieving services with slots");
        }
    }
}