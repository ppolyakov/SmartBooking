using SmartBooking.BlazorUI.Models;
using SmartBooking.BlazorUI.Services.Interfaces;

namespace SmartBooking.BlazorUI.Services;

public class ClientService(HttpClient httpClient, ILogger<ClientService> logger) : IClientService
{
    private readonly ILogger<ClientService> _logger = logger;

    public async Task<Guid> CreateClientAsync(string name, string email)
    {
        var client = new ClientDto { Name = name, Email = email };
        var response = await httpClient.PostAsJsonAsync("clients", client);

        if (response.IsSuccessStatusCode)
        {
            var created = await response.Content.ReadFromJsonAsync<ClientDto>();
            return created?.Id ?? Guid.Empty;
        }

        _logger.LogError("Failed to create client. Status code: {StatusCode}", response.StatusCode);
        return Guid.Empty;
    }
}