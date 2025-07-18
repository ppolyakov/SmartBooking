namespace SmartBooking.WebAPI.Models;

public class TimeSlotWithClientDto
{
    public Guid Id { get; set; }
    public DateTime StartTime { get; set; }
    public bool IsBooked { get; set; }
    public string? ClientEmail { get; set; }
}