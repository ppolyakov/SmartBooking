namespace SmartBooking.WebAPI.Models;

public class BookingDto
{
    public Guid Id { get; set; }
    public DateTime StartTime { get; set; }
    public string ServiceTitle { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public string ClientEmail { get; set; } = string.Empty;
}