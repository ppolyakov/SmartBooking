namespace SmartBooking.WebAPI.Models;

public class TimeSlotDto
{
    public Guid Id { get; set; }
    public DateTime StartTime { get; set; }
    public string ServiceTitle { get; set; } = string.Empty;
    public bool IsBooked { get; set; }
}