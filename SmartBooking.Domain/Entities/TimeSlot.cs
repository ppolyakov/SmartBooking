using SmartBooking.Domain.Common;

namespace SmartBooking.Domain.Entities;

public class TimeSlot : BaseEntity
{
    public DateTime StartTime { get; set; }
    public Guid ServiceId { get; set; }
    public Service Service { get; set; } = default!;

    public Booking? Booking { get; set; }
}