using AutoMapper;
using Booking.Application.DTOs.Bookings;
using Booking.Application.Interfaces.Repository;
using Booking.Application.Interfaces.Services;
using Booking.Domain.Entities;

namespace Booking.Application.Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IMapper _mapper;

    public BookingService(
        IBookingRepository bookingRepository,
        IRoomRepository roomRepository,
        IMapper mapper)
    {
        _bookingRepository = bookingRepository;
        _roomRepository = roomRepository;
        _mapper = mapper;
    }

    public async Task<BookingDto> CreateAsync(
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

        if (dto.ChildrenCount < 0)
        {
            throw new Exception(
                "Children count cannot be negative");
        }

        Room? room =
            await _roomRepository.GetByIdAsync(
                dto.RoomId,
                cancellationToken);

        if (room == null ||
            !room.IsAvailable)
        {
            throw new Exception(
                "Room is not available");
        }

        if (room.Capacity <
            dto.AdultsCount + dto.ChildrenCount)
        {
            throw new Exception(
                "Room capacity is not enough for selected guests");
        }

        bool isBooked =
            await _bookingRepository.RoomIsBookedAsync(
                dto.RoomId,
                dto.CheckInDate,
                dto.CheckOutDate,
                cancellationToken);

        if (isBooked)
        {
            throw new Exception(
                "Room is already booked for these dates");
        }

        int nights =
            (dto.CheckOutDate.Date -
             dto.CheckInDate.Date).Days;

        BookingEntity booking =
            new BookingEntity
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

        BookingEntity? savedBooking =
            await _bookingRepository.GetByIdAsync(
                booking.Id,
                cancellationToken);

        if (savedBooking == null)
        {
            throw new Exception(
                "Booking was created but could not be loaded");
        }

        return _mapper.Map<BookingDto>(
            savedBooking);
    }

    public async Task<List<BookingDto>> GetMyBookingsAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        List<BookingEntity> bookings =
            await _bookingRepository.GetByUserIdAsync(
                userId,
                cancellationToken);

        return _mapper.Map<List<BookingDto>>(
            bookings);
    }
}
