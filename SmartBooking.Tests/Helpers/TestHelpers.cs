using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using SmartBooking.Domain.Entities;
using SmartBooking.Infrastructure.Persistence;

namespace SmartBooking.Tests.Helpers;

public static class TestHelpers
{
    public static AppDbContext CreateInMemoryDb(string dbName = null)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName ?? Guid.NewGuid().ToString())
            .Options;
        var db = new AppDbContext(options);

        // seed a Service and a TimeSlot
        var service = new Service
        {
            Title = "Test Service",
            Duration = 30,
            Date = DateTime.Today,
            Description = "Desc",
            Price = 10m
        };
        db.Services.Add(service);
        db.SaveChanges();

        var slot = new TimeSlot
        {
            ServiceId = service.Id,
            StartTime = DateTime.Today.AddHours(9)
        };
        db.TimeSlots.Add(slot);
        db.SaveChanges();

        return db;
    }

    public static Mock<UserManager<ApplicationUser>> MockUserManager(ApplicationUser user)
    {
        var store = new Mock<IUserStore<ApplicationUser>>();
        var mgr = new Mock<UserManager<ApplicationUser>>(
            store.Object, null, null, null, null, null, null, null, null);

        mgr.Setup(x => x.FindByIdAsync(user.Id))
           .ReturnsAsync(user);
        mgr.Setup(x => x.GetRolesAsync(user))
           .ReturnsAsync(new List<string> { "User" });

        return mgr;
    }

    public static NullLogger<BookingService> MockLogger() =>
        NullLogger<BookingService>.Instance;
}