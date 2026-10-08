using AutoMapper;
using Booking.Application.DTOs.Hotels;
using Booking.Domain.Entities;

namespace Booking.Application.Mapping;

public class HotelProfile : Profile
{
    public HotelProfile()
    {
        CreateMap<Hotel, HotelFavoriteDto>()
            .ForMember(
                dest => dest.Rating,
                opt => opt.MapFrom(src =>
                src.Reviews.Any()
                ? src.Reviews.Average(x => x.Rating)
                : 0))
             .ForMember(
                 dest => dest.Images,
                 opt => opt.MapFrom(src =>
                     src.Images.Select(x => x.ImageUrl).ToList()))
             .ForMember(
                 dest => dest.Rooms,
                 opt => opt.MapFrom(src => src.Rooms));

        CreateMap<HotelCreateDto, Hotel>()
            .ForMember(
                destination => destination.Id,
                options => options.Ignore())
            .ForMember(
                destination => destination.Amenities,
                options => options.Ignore())
            .ForMember(
                destination => destination.Rooms,
                options => options.Ignore())
            .ForMember(
                destination => destination.Images,
                options => options.Ignore())
            .ForMember(
                destination => destination.Reviews,
                options => options.Ignore());

        CreateMap<Hotel, HotelDto>()
            .ForMember(
                destination => destination.Facilities,
                options => options.Ignore())
            .ForMember(
                destination => destination.Staff,
                options => options.Ignore())
            .ForMember(
                destination => destination.Cleanliness,
                options => options.Ignore())
            .ForMember(
                destination => destination.Comfort,
                options => options.Ignore())
            .ForMember(
                destination => destination.Location,
                options => options.Ignore())
            .ForMember(
                destination => destination.ValueForMoney,
                options => options.Ignore())
            .ForMember(
                destination => destination.Rating,
                options => options.Ignore())
            .ForMember(
                destination => destination.ReviewsCount,
                options => options.Ignore())
            .ForMember(
                destination => destination.MainImageUrl,
                options => options.Ignore())
            .ForMember(
                destination => destination.Images,
                options => options.Ignore())
            .ForMember(
                destination => destination.Rooms,
                options => options.Ignore())
            .ForMember(
                destination => destination.Amenities,
                options => options.Ignore())
            .ForMember(
                destination => destination.HasWifi,
                options => options.Ignore())
            .ForMember(
                destination => destination.Reviews,
                options => options.Ignore())
            .ForMember(
                destination => destination.MapUrl,
                options => options.Ignore());
    }
}