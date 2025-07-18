namespace SmartBooking.Shared.Http.Requests;

public class BookSlotRequest
{
    public Guid SlotId { get; set; }
    public Guid UserId { get; set; }
}