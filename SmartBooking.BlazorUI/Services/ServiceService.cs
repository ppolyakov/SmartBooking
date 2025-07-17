using SmartBooking.BlazorUI.Models;

namespace SmartBooking.BlazorUI.Services;

public class ServiceService : IServiceService
{
    private readonly HttpClient _http;

    public ServiceService(IHttpClientFactory factory)
    {
        _http = factory.CreateClient("SmartBookingAPI");
    }

    public async Task<List<ServiceDto>> GetAllServicesAsync()
    {
        return await _http.GetFromJsonAsync<List<ServiceDto>>("services") ?? [];
    }

    public async Task<bool> CreateServiceAsync(ServiceDto dto)
    {
        var response = await _http.PostAsJsonAsync("services", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> GenerateSlotsAsync(Guid serviceId, DateTime date)
    {
        var url = $"timeslots/generate?serviceId={serviceId}&date={date:yyyy-MM-dd}";
        var response = await _http.PostAsync(url, null);
        return response.IsSuccessStatusCode;
    }
}