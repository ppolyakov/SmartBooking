namespace SmartBooking.BlazorUI.Services.Interfaces;

public interface IClientService
{
    Task<Guid> CreateClientAsync(string name, string email);
}