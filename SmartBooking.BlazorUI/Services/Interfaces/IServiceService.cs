using SmartBooking.BlazorUI.Helpers;
using SmartBooking.Shared;
using SmartBooking.Shared.Dto;

namespace SmartBooking.BlazorUI.Services.Interfaces;

public interface IServiceService
{
    Task<Result<List<ServiceDto>>> GetAllServicesAsync();
    Task<Result<bool>> CreateServiceAsync(ServiceDto dto);
    Task<Result<bool>> GenerateSlotsAsync(Guid serviceId, DateTime date);
    Task<Result<List<ServiceWithSlotsDto>>> GetServicesWithSlotsAsync();
}