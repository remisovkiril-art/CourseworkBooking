using System;
using System.Collections.Generic;

namespace Booking.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string TravelPurpose { get; set; } = string.Empty;
    public string VerificationCode { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public bool TravelingWithPet { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }

    public ICollection<BookingEntity> Bookings { get; set; } = new List<BookingEntity>();
    public ICollection<PaymentMethod> PaymentMethods { get; set; } = new List<PaymentMethod>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}
