using SmartBooking.Domain.Entities;
using SmartBooking.Infrastructure.Persistence;

namespace SmartBooking.Infrastructure.Seed;

public static class DbInitializer
{
    public static void Seed(AppDbContext db)
    {
        if (db.Services.Any()) return;

        var service = new Service
        {
            Title = "Haircut",
            Duration = TimeSpan.FromMinutes(30)
        };

        db.Services.Add(service);

        var client = new Client
        {
            Name = "Jhonny Depp",
            Email = "johnydepp@example.com"
        };

        db.Clients.Add(client);
        db.SaveChanges();
    }
}