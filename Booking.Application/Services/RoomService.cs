using AutoMapper;
using Booking.Application.DTOs.Hotels;
using Booking.Application.Interfaces.Repository;
using Booking.Application.Interfaces.Services;
using Booking.Domain.Entities;

namespace Booking.Application.Services;

public class RoomService : IRoomService
{
    private readonly IRoomRepository _roomRepository;
    private readonly IHotelRepository _hotelRepository;
    private readonly IMapper _mapper;
    private readonly ICachingService _cachingService;

    public RoomService(
        IRoomRepository roomRepository,
        IHotelRepository hotelRepository,
        IMapper mapper,
        ICachingService cachingService
        )
    {
        _roomRepository = roomRepository;
        _hotelRepository = hotelRepository;
        _mapper = mapper;
        _cachingService = cachingService;
    }

    public async Task<List<RoomDto>> GetByHotelIdAsync(
        Guid hotelId,
        CancellationToken cancellationToken)
    {
        var cacheKey = $"rooms:hotel:{hotelId}";
        var cache = await _cachingService.GetAsync<List<RoomDto>>(cacheKey);
        if (cache == null)
        {
            var rooms = await _roomRepository.GetByHotelIdAsync(
            hotelId,
            cancellationToken);
            cache = _mapper.Map<List<RoomDto>>(rooms);
            await _cachingService.SetAsync(cacheKey, cache, null);

        }
        return cache;
        //var rooms = await _roomRepository.GetByHotelIdAsync(
        //    hotelId,
        //    cancellationToken);

        //return _mapper.Map<List<RoomDto>>(rooms);
    }

    public async Task<RoomDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var room = await _roomRepository.GetByIdAsync(
            id,
            cancellationToken);
        return room == null ? null : _mapper.Map<RoomDto>(room);
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

        var room = _mapper.Map<Room>(dto);
        room.Id = Guid.NewGuid();
        room.HotelId = hotelId;

        await _roomRepository.AddAsync(room, cancellationToken);
        await _cachingService.RemoveAsync($"rooms:hotel:{hotelId}");
        return _mapper.Map<RoomDto>(room);
    }

}