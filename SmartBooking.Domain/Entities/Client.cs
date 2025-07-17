using SmartBooking.Domain.Common;

namespace SmartBooking.Domain.Entities;

public class Client : BaseEntity
{
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;

    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}