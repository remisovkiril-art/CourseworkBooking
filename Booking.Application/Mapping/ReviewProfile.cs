using AutoMapper;
using Booking.Application.DTOs.Reviews;
using Booking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Application.Mapping;

public class ReviewProfile : Profile
{
    public ReviewProfile()
    {
        CreateMap<Review, ReviewDto>()
            .ForMember(dest => dest.AuthorName,opt => opt.MapFrom(src => src.User.Name));

        CreateMap<CreateReviewDto, Review>();
    }
}
