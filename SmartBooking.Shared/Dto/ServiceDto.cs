namespace SmartBooking.BlazorUI.Models;

public class ServiceDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public TimeSpan Duration { get; set; }
}