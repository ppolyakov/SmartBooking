using SmartBooking.BlazorUI.Models;

namespace SmartBooking.BlazorUI.Services;

public class ClientService : IClientService
{
    private readonly HttpClient _http;

    public ClientService(IHttpClientFactory factory)
    {
        _http = factory.CreateClient("SmartBookingAPI");
    }

    public async Task<Guid> CreateClientAsync(string name, string email)
    {
        var client = new ClientDto { Name = name, Email = email };
        var response = await _http.PostAsJsonAsync("clients", client);

        if (response.IsSuccessStatusCode)
        {
            var created = await response.Content.ReadFromJsonAsync<ClientDto>();
            return created?.Id ?? Guid.Empty;
        }

        return Guid.Empty;
    }
}