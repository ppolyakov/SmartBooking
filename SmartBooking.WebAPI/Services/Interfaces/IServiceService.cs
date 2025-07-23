using SmartBooking.Domain.Entities;
using SmartBooking.Shared;
using SmartBooking.Shared.Dto;

namespace SmartBooking.WebAPI.Services.Interfaces;

public interface IServiceService
{
    Task<Result<IEnumerable<Service>>> GetAllServicesAsync();
    Task<Result<Service>> CreateServiceAsync(Service service);
    Task<Result<List<ServiceWithSlotsDto>>> GetAllServicesWithSlotsAsync();
}