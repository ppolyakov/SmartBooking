namespace SmartBooking.BlazorUI.Services;

public interface IClientService
{
    Task<Guid> CreateClientAsync(string name, string email);
}