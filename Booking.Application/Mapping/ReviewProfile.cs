using AutoMapper;
using Booking.Application.DTOs.Reviews;
using Booking.Domain.Entities;

namespace Booking.Application.Mapping;

public class ReviewProfile : Profile
{
    public ReviewProfile()
    {
        CreateMap<CreateReviewDto, Review>()
            .ForMember(
                destination => destination.Comment,
                options => options.MapFrom(source => source.Text)
            );

        CreateMap<Review, ReviewDto>()
            .ForMember(
                dest => dest.AuthorName,
                opt => opt.MapFrom(src =>
                    src.User != null
                        ? src.User.Name
                        : "User"))
            .ForMember(
                dest => dest.AuthorAvatarUrl,
                opt => opt.MapFrom(src =>
                    src.User != null
                        ? src.User.AvatarUrl
                        : null))
            .ForMember(
                dest => dest.Text,
                opt => opt.MapFrom(src => src.Comment));
    }
}