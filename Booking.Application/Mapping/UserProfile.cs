using AutoMapper;
using Booking.Application.DTOs.Auth;
using Booking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Application.Mapping;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, AuthResponseDto>()
            .ForMember(dest => dest.AccessToken, opt => opt.Ignore())
            .ForMember(dest => dest.AccessTokenExpires, opt => opt.Ignore())
            .ForMember(dest => dest.VerificationCode, opt => opt.Ignore());

        CreateMap<User, UpdateUserDto>();

        CreateMap<UpdateUserDto, User>();

    }
}
