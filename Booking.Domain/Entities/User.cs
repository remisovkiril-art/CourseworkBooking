using System;
using System.Collections.Generic;

namespace Booking.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Country { get; set; }
    public string? City { get; set; }
    public string? TravelPurpose { get; set; }
    public string VerificationCode { get; set; } = string.Empty;
    public bool IsVerified { get; set; }
    public bool? TravelingWithPet { get; set; }
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime RefreshTokenExpiryTime { get; set; }

    public ICollection<BookingEntity> Bookings { get; set; } = new List<BookingEntity>();
    public ICollection<PaymentMethod> PaymentMethods { get; set; } = new List<PaymentMethod>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}
