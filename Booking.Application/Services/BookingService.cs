using Booking.Application.DTOs.Bookings;
using Booking.Application.Interfaces.Repository;
using Booking.Application.Interfaces.Services;
using Booking.Domain.Entities;

namespace Booking.Application.Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IRoomRepository _roomRepository;

    public BookingService(
        IBookingRepository bookingRepository,
        IRoomRepository roomRepository)
    {
        _bookingRepository = bookingRepository;
        _roomRepository = roomRepository;
    }

    public async Task<object> CreateAsync(
        Guid userId,
        CreateBookingDto dto,
        CancellationToken cancellationToken)
    {
        if (dto.CheckOutDate <= dto.CheckInDate)
        {
            throw new Exception(
                "Check-out date must be after check-in date");
        }

        if (dto.AdultsCount < 1)
        {
            throw new Exception(
                "At least one adult is required");
        }

        var room = await _roomRepository.GetByIdAsync(
            dto.RoomId,
            cancellationToken);

        if (room == null || !room.IsAvailable)
        {
            throw new Exception(
                "Room is not available");
        }

        var isBooked = await _bookingRepository.RoomIsBookedAsync(
            dto.RoomId,
            dto.CheckInDate,
            dto.CheckOutDate,
            cancellationToken);

        if (isBooked)
        {
            throw new Exception(
                "Room is already booked for these dates");
        }

        var nights =
            (dto.CheckOutDate.Date -
             dto.CheckInDate.Date).Days;

        var booking = new BookingEntity
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            RoomId = dto.RoomId,
            CheckInDate = dto.CheckInDate,
            CheckOutDate = dto.CheckOutDate,
            AdultsCount = dto.AdultsCount,
            ChildrenCount = dto.ChildrenCount,
            TravelDetails = dto.TravelDetails,
            TotalPrice =
                room.PricePerNight * nights,
            IsPaid = dto.IsPaid,
            Status = dto.IsPaid
                ? "Confirmed"
                : "Pending",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _bookingRepository.AddAsync(
            booking,
            cancellationToken);

        return new
        {
            booking.Id,
            booking.RoomId,
            booking.CheckInDate,
            booking.CheckOutDate,
            Nights = nights,
            booking.Status
        };
    }

    public async Task<List<object>> GetMyBookingsAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var bookings =
            await _bookingRepository.GetByUserIdAsync(
                userId,
                cancellationToken);

        return bookings
            .Select(x => (object)new
            {
                x.Id,
                x.RoomId,
                x.CheckInDate,
                x.CheckOutDate,
                x.AdultsCount,
                x.ChildrenCount,
                x.TotalPrice,
                x.Status
            })
            .ToList();
    }
}


