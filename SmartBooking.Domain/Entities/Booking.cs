using SmartBooking.Domain.Common;

namespace SmartBooking.Domain.Entities;

public class Booking : BaseEntity
{
    public Guid ClientId { get; set; }
    public Client Client { get; set; } = default!;

    public Guid TimeSlotId { get; set; }
    public TimeSlot TimeSlot { get; set; } = default!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}