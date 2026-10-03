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
                destination => destination.Text,
                options => options.MapFrom(source => source.Comment)
            )
            .ForMember(
                destination => destination.AuthorName,
                options => options.MapFrom(source => source.User.Name)
            );
    }
}