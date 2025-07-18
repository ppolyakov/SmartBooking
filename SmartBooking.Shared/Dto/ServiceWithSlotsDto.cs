namespace SmartBooking.Shared.Dto;

public class ServiceWithSlotsDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public TimeSpan Duration { get; set; }
    public List<TimeSlotWithClientDto> Slots { get; set; } = new();
}