namespace SmartBooking.BlazorUI.Services.Interfaces;

public interface ILoginService
{
    Task<bool> LoginAsync(string email, string password);
}