using AutoMapper;
using Booking.Application.DTOs.Hotels;
using Booking.Domain.Entities;

namespace Booking.Application.Mapping;

public class HotelProfile : Profile
{
    public HotelProfile()
    {
        CreateMap<HotelCreateDto, Hotel>()
            .ForMember(dest => dest.Amenities, opt => opt.Ignore())
            .ForMember(dest => dest.Rooms, opt => opt.Ignore());
    }
}