using AutoMapper;
using Booking.Application.DTOs.Auth;
using Booking.Domain.Entities;

namespace Booking.Application.Mapping;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, AuthResponseDto>()
            .ForMember(
                destination => destination.AccessToken,
                options => options.Ignore())
            .ForMember(
                destination => destination.AccessTokenExpires,
                options => options.Ignore())
            .ForMember(
                destination => destination.RefreshToken,
                options => options.MapFrom(
                    source => source.RefreshToken))
            .ForMember(
                destination => destination.Role,
                options => options.MapFrom(
                    source => source.Role));

        CreateMap<User, UpdateUserDto>();

        CreateMap<UpdateUserDto, User>()
            .ForMember(
                destination => destination.Id,
                options => options.Ignore())
            .ForMember(
                destination => destination.Email,
                options => options.Ignore())
            .ForMember(
                destination => destination.PasswordHash,
                options => options.Ignore())
            .ForMember(
                destination => destination.VerificationCode,
                options => options.Ignore())
            .ForMember(
                destination => destination.VerificationCodeExpiresAt,
                options => options.Ignore())
            .ForMember(
                destination => destination.VerificationPurpose,
                options => options.Ignore())
            .ForMember(
                destination => destination.RefreshToken,
                options => options.Ignore())
            .ForMember(
                destination => destination.RefreshTokenExpiryTime,
                options => options.Ignore())
            .ForMember(
                destination => destination.Role,
                options => options.Ignore())
            .ForMember(
                destination => destination.IsVerified,
                options => options.Ignore())
            .ForMember(
                destination => destination.Bookings,
                options => options.Ignore())
            .ForMember(
                destination => destination.PaymentMethods,
                options => options.Ignore())
            .ForMember(
                destination => destination.Reviews,
                options => options.Ignore())
            .ForMember(
                destination => destination.TravelPreferences,
                options => options.Ignore())
            .ForMember(
                destination => destination.NewsletterSubscription,
                options => options.Ignore());
    }
}