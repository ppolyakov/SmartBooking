using SmartBooking.BlazorUI.Models;

namespace SmartBooking.BlazorUI.Services.Interfaces;

public interface IServiceService
{
    Task<List<ServiceDto>> GetAllServicesAsync();
    Task<bool> CreateServiceAsync(ServiceDto dto);
    Task<bool> GenerateSlotsAsync(Guid serviceId, DateTime date);
    Task<List<ServiceWithSlotsDto>> GetServicesWithSlotsAsync();
}