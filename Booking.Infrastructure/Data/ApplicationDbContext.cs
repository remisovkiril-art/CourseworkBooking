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

    public DbSet<HotelAmenity> HotelAmenities =>
        Set<HotelAmenity>();

    public DbSet<HotelImage> HotelImages =>
        Set<HotelImage>();

    public DbSet<Room> Rooms => Set<Room>();

    public DbSet<Review> Reviews => Set<Review>();

    public DbSet<BookingEntity> Bookings =>
        Set<BookingEntity>();

    public DbSet<PaymentMethod> PaymentMethods =>
        Set<PaymentMethod>();

    public DbSet<UserTravelPreference> UserTravelPreferences =>
        Set<UserTravelPreference>();

    public DbSet<NewsletterSubscription> NewsletterSubscriptions =>
        Set<NewsletterSubscription>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<BookingEntity>()
            .Property(x => x.TotalPrice)
            .HasPrecision(18, 2);

        modelBuilder.Entity<HotelImage>()
            .HasOne(x => x.Hotel)
            .WithMany(x => x.Images)
            .HasForeignKey(x => x.HotelId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Review>()
            .HasOne(x => x.Hotel)
            .WithMany(x => x.Reviews)
            .HasForeignKey(x => x.HotelId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Review>()
            .HasOne(x => x.User)
            .WithMany(x => x.Reviews)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<HotelAmenity>()
            .HasOne(x => x.Hotel)
            .WithMany(x => x.Amenities)
            .HasForeignKey(x => x.HotelId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Room>()
            .HasOne(x => x.Hotel)
            .WithMany(x => x.Rooms)
            .HasForeignKey(x => x.HotelId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserTravelPreference>()
            .HasOne(x => x.User)
            .WithMany(x => x.TravelPreferences)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<NewsletterSubscription>()
            .HasOne(x => x.User)
            .WithOne(x => x.NewsletterSubscription)
            .HasForeignKey<NewsletterSubscription>(
                x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}