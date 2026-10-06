using AutoMapper;
using Booking.Application.DTOs.Hotels;
using Booking.Domain.Entities;

namespace Booking.Application.Mapping;

public class RoomProfile : Profile
{
    public RoomProfile()
    {
        CreateMap<Room, RoomDto>();

        CreateMap<RoomCreateDto, Room>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Hotel, opt => opt.Ignore())
            .ForMember(dest => dest.Bookings, opt => opt.Ignore());
    }
}