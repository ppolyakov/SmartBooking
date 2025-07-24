namespace SmartBooking.Shared.Dto;

public class ServiceWithSlotsDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Duration { get; set; }
    public DateTime Date { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }

    public List<TimeSlotWithClientDto> Slots { get; set; } = new();
}