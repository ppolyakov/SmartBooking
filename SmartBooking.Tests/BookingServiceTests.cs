using Microsoft.EntityFrameworkCore;
using Moq;
using SmartBooking.Infrastructure.Persistence;
using SmartBooking.Shared.Http.Requests;
using SmartBooking.Tests.Helpers;
using SmartBooking.WebAPI.Services.Interfaces;

namespace SmartBooking.Tests;

public class BookingServiceTests
{
    [Fact]
    public async Task CreateBooking_SendsEmail()
    {
        // Arrange
        var db = TestHelpers.CreateInMemoryDb();
        var service = await db.Services.FirstAsync();
        var slot = await db.TimeSlots.FirstAsync();
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "testuser",
            Email = "test@example.com"
        };

        var userManagerMock = TestHelpers.MockUserManager(user);

        var notifMock = new Mock<INotificationService>();
        var svc = new BookingService(db, notifMock.Object, userManagerMock.Object, TestHelpers.MockLogger());

        var request = new BookSlotRequest
        {
            UserId = Guid.Parse(user.Id),
            SlotId = slot.Id
        };

        // Act
        var result = await svc.CreateBookingAsync(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        notifMock.Verify(x => x.SendBookingConfirmationAsync(
            user.Email,
            user.UserName,
            result.Value.Id,
            slot.StartTime,
            service.Title,
            CancellationToken.None),
            Times.Once);
    }
}