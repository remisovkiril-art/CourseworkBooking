using AutoMapper;
using Booking.Application.DTOs.Bookings;
using Booking.Domain.Entities;

namespace Booking.Application.Mapping;

public class BookingProfile : Profile
{
    public BookingProfile()
    {
        CreateMap<BookingEntity, BookingDto>()
            .ForMember(
                destination => destination.HotelId,
                options => options.MapFrom(
                    source => source.Room.HotelId))
            .ForMember(
                destination => destination.HotelName,
                options => options.MapFrom(
                    source => source.Room.Hotel.Name))
            .ForMember(
                destination => destination.HotelAddress,
                options => options.MapFrom(
                    source => source.Room.Hotel.Address))
            .ForMember(
                destination => destination.RoomTitle,
                options => options.MapFrom(
                    source => source.Room.Title))
            .ForMember(
                destination => destination.Nights,
                options => options.MapFrom(
                    source =>
                        (source.CheckOutDate -
                         source.CheckInDate).Days));
    }
}