using SmartBooking.Domain.Entities;
using SmartBooking.Shared;
using SmartBooking.Shared.Dto;
using SmartBooking.Shared.Http.Requests;

namespace SmartBooking.WebAPI.Services.Interfaces;

public interface IBookingService
{
    Task<Result<IEnumerable<BookingDto>>> GetAllBookingsAsync(CancellationToken ct = default);
    Task<Result<BookingDto>> CreateBookingAsync(BookSlotRequest request, CancellationToken ct = default);
}