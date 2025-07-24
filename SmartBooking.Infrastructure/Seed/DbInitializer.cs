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
            Duration = 30,
            Date = DateTime.Today,
            Description = "Professional haircut service",
            Price = 25.00m
        };
        var massage = new Service
        {
            Title = "Massage",
            Duration = 60,
            Date = DateTime.Today,
            Description = "Relaxing full-body massage",
            Price = 50.00m
        };
        db.Services.AddRange(haircut, massage);
        db.SaveChanges();

        var slots = new List<TimeSlot>();
        for (int day = 0; day < 3; day++)
        {
            var baseTime = DateTime.Today.AddDays(day).AddHours(9);
            for (int i = 0; i < 8; i++)
            {
                slots.Add(new TimeSlot
                {
                    ServiceId = haircut.Id,
                    StartTime = baseTime.AddMinutes(haircut.Duration * i)
                });
                slots.Add(new TimeSlot
                {
                    ServiceId = massage.Id,
                    StartTime = baseTime.AddMinutes(massage.Duration * i)
                });
            }
        }
        db.TimeSlots.AddRange(slots);
        db.SaveChanges();
    }
}