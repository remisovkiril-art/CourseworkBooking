using Booking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Booking.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Hotel> Hotels => Set<Hotel>();
    public DbSet<HotelAmenity> HotelAmenities => Set<HotelAmenity>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<BookingEntity> Bookings => Set<BookingEntity>();
    public DbSet<PaymentMethod> PaymentMethods => Set<PaymentMethod>();
    public DbSet<Review> Reviews => Set<Review>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<BookingEntity>()
            .Property(x => x.TotalPrice)
            .HasPrecision(18, 2);
    }
}