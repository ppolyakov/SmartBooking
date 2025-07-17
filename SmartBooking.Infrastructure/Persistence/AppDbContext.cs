using Microsoft.EntityFrameworkCore;
using SmartBooking.Domain.Entities;

namespace SmartBooking.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Service> Services => Set<Service>();
    public DbSet<TimeSlot> TimeSlots => Set<TimeSlot>();
    public DbSet<Booking> Bookings => Set<Booking>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TimeSlot>()
            .HasOne(t => t.Booking)
            .WithOne(b => b.TimeSlot)
            .HasForeignKey<Booking>(b => b.TimeSlotId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}