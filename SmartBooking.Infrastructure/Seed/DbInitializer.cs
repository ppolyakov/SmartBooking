using SmartBooking.Domain.Entities;
using SmartBooking.Infrastructure.Persistence;

namespace SmartBooking.Infrastructure.Seed;

public static class DbInitializer
{
    public static void Seed(AppDbContext db)
    {
        if (db.Services.Any())
            return;

        var haircut = new Service
        {
            Title = "Haircut",
            Duration = TimeSpan.FromMinutes(30)
        };
        var massage = new Service
        {
            Title = "Massage",
            Duration = TimeSpan.FromHours(1)
        };
        db.Services.AddRange(haircut, massage);
        db.SaveChanges();

        var slots = new List<TimeSlot>();
        for (int day = 0; day < 3; day++)
        {
            var date = DateTime.Today.AddDays(day).AddHours(9);
            for (int i = 0; i < 8; i++)
            {
                slots.Add(new TimeSlot
                {
                    ServiceId = haircut.Id,
                    StartTime = date.AddHours(i)
                });
                slots.Add(new TimeSlot
                {
                    ServiceId = massage.Id,
                    StartTime = date.AddHours(i)
                });
            }
        }
        db.TimeSlots.AddRange(slots);
        db.SaveChanges();
    }
}