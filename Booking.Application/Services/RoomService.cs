using Booking.Application.DTOs.Hotels;
using Booking.Application.Interfaces.Repository;
using Booking.Application.Interfaces.Services;
using Booking.Domain.Entities;

namespace Booking.Application.Services;

public class RoomService : IRoomService
{
    private readonly IRoomRepository _roomRepository;
    private readonly IHotelRepository _hotelRepository;

    public RoomService(
        IRoomRepository roomRepository,
        IHotelRepository hotelRepository)
    {
        _roomRepository = roomRepository;
        _hotelRepository = hotelRepository;
    }

    public async Task<List<RoomDto>> GetByHotelIdAsync(
        Guid hotelId,
        CancellationToken cancellationToken)
    {
        var rooms = await _roomRepository.GetByHotelIdAsync(
            hotelId,
            cancellationToken);

        return rooms.Select(Map).ToList();
    }

    public async Task<RoomDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var room = await _roomRepository.GetByIdAsync(
            id,
            cancellationToken);

        return room == null ? null : Map(room);
    }

    public async Task<RoomDto> CreateAsync(
        Guid hotelId,
        RoomCreateDto dto,
        CancellationToken cancellationToken)
    {
        var hotel = await _hotelRepository.GetByIdAsync(
            hotelId,
            cancellationToken);

        if (hotel == null)
            throw new Exception("Hotel not found");

        var count = await _roomRepository.CountByHotelIdAsync(
            hotelId,
            cancellationToken);

        if (count >= 4)
            throw new Exception("A hotel can have maximum 4 rooms");

        if (string.IsNullOrWhiteSpace(dto.Title))
            throw new Exception("Room title is required");

        if (dto.Capacity < 1)
            throw new Exception("Room capacity must be at least 1");

        if (dto.PricePerNight < 0)
            throw new Exception("Room price cannot be negative");

        var room = new Room
        {
            Id = Guid.NewGuid(),
            HotelId = hotelId,
            Title = dto.Title,
            BedType = dto.BedType,
            Capacity = dto.Capacity,
            PricePerNight = dto.PricePerNight,
            IsAvailable = dto.IsAvailable,
            ImageUrl = dto.ImageUrl
        };

        await _roomRepository.AddAsync(room, cancellationToken);

        return Map(room);
    }

    private static RoomDto Map(Room room)
    {
        return new RoomDto
        {
            Id = room.Id,
            HotelId = room.HotelId,
            Title = room.Title,
            BedType = room.BedType,
            Capacity = room.Capacity,
            PricePerNight = room.PricePerNight,
            IsAvailable = room.IsAvailable,
            ImageUrl = room.ImageUrl
        };
    }
}