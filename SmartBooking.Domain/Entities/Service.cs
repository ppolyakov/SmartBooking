using SmartBooking.Domain.Common;

namespace SmartBooking.Domain.Entities;

public class Service : BaseEntity
{
    public string Title { get; set; } = default!;
    public TimeSpan Duration { get; set; }

    public ICollection<TimeSlot> TimeSlots { get; set; } = new List<TimeSlot>();
}