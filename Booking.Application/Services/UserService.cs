using AutoMapper;
using Booking.Application.DTOs.Auth;
using Booking.Application.Interfaces.Repository;
using Booking.Application.Interfaces.Services;
using Booking.Domain.Entities;

namespace Booking.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserService(
        IUserRepository userRepository,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<UpdateUserDto?> GetAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        User? user = await _userRepository.GetByIdAsync(
            userId,
            cancellationToken);

        if (user == null)
        {
            return null;
        }

        return _mapper.Map<UpdateUserDto>(user);
    }

    public async Task UpdateAsync(
        Guid userId,
        UpdateUserDto dto,
        string? avatarUrl,
        CancellationToken cancellationToken)
    {
        User? user = await _userRepository.GetByIdAsync(
            userId,
            cancellationToken);

        if (user == null)
        {
            throw new Exception("User not found");
        }

        if (!string.IsNullOrWhiteSpace(dto.Email))
        {
            string newEmail = dto.Email.Trim();

            if (!string.Equals(
                    user.Email,
                    newEmail,
                    StringComparison.OrdinalIgnoreCase))
            {
                User? existingUser =
                    await _userRepository.GetByEmailAsync(
                        newEmail,
                        cancellationToken);

                if (existingUser != null &&
                    existingUser.Id != userId)
                {
                    throw new Exception(
                        "User with this email already exists");
                }

                user.Email = newEmail;
                user.IsVerified = false;
            }
        }

        user.Name = dto.Name;
        user.Phone = dto.Phone;
        user.Country = dto.Country;
        user.City = dto.City;
        user.PreferredCurrency = dto.PreferredCurrency;
        user.TravelPurpose = dto.TravelPurpose;
        user.TravelingWithPet = dto.TravelingWithPet;
        user.Gender = dto.Gender;
        user.DateOfBirth = dto.DateOfBirth;

        if (avatarUrl != null)
        {
            user.AvatarUrl = avatarUrl;
        }

        await _userRepository.UpdateAsync(
            user,
            cancellationToken);
    }

    public async Task<TravelPreferencesDto> GetTravelPreferencesAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        List<UserTravelPreference> preferences =
            await _userRepository.GetTravelPreferencesAsync(
                userId,
                cancellationToken);

        TravelPreferencesDto result =
            new TravelPreferencesDto();

        foreach (UserTravelPreference preference in preferences)
        {
            AddPreference(
                result,
                preference.Category,
                preference.PreferenceName);
        }

        return result;
    }

    public async Task SaveTravelPreferencesAsync(
        Guid userId,
        TravelPreferencesDto dto,
        CancellationToken cancellationToken)
    {
        List<UserTravelPreference> preferences =
            new List<UserTravelPreference>();

        AddPreferences(
            preferences,
            userId,
            "General",
            dto.General);

        AddPreferences(
            preferences,
            userId,
            "Accessibility",
            dto.Accessibility);

        AddPreferences(
            preferences,
            userId,
            "Languages",
            dto.Languages);

        AddPreferences(
            preferences,
            userId,
            "Parking",
            dto.Parking);

        AddPreferences(
            preferences,
            userId,
            "ReceptionServices",
            dto.ReceptionServices);

        AddPreferences(
            preferences,
            userId,
            "CleaningServices",
            dto.CleaningServices);

        AddPreferences(
            preferences,
            userId,
            "EntertainmentAndFamily",
            dto.EntertainmentAndFamily);

        AddPreferences(
            preferences,
            userId,
            "SafetyAndSecurity",
            dto.SafetyAndSecurity);

        await _userRepository.ReplaceTravelPreferencesAsync(
            userId,
            preferences,
            cancellationToken);
    }

    public async Task<NewsletterDto?> GetNewsletterAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        NewsletterSubscription? newsletter =
            await _userRepository.GetNewsletterAsync(
                userId,
                cancellationToken);

        if (newsletter == null)
        {
            return null;
        }

        return new NewsletterDto
        {
            Email = newsletter.Email,
            SeasonalOffers = newsletter.SeasonalOffers,
            FavoriteCities = newsletter.FavoriteCities,
            AcrossTheWorld = newsletter.AcrossTheWorld,
            AffordableTravel = newsletter.AffordableTravel
        };
    }

    public async Task SaveNewsletterAsync(
        Guid userId,
        NewsletterDto dto,
        CancellationToken cancellationToken)
    {
        NewsletterSubscription newsletter =
            new NewsletterSubscription
            {
                UserId = userId,
                Email = dto.Email,
                SeasonalOffers = dto.SeasonalOffers,
                FavoriteCities = dto.FavoriteCities,
                AcrossTheWorld = dto.AcrossTheWorld,
                AffordableTravel = dto.AffordableTravel
            };

        await _userRepository.SaveNewsletterAsync(
            newsletter,
            cancellationToken);
    }

    private static void AddPreferences(
        List<UserTravelPreference> preferences,
        Guid userId,
        string category,
        List<string> values)
    {
        foreach (string value in values)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                continue;
            }

            preferences.Add(
                new UserTravelPreference
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Category = category,
                    PreferenceName = value.Trim()
                });
        }
    }

    private static void AddPreference(
        TravelPreferencesDto dto,
        string category,
        string value)
    {
        if (category == "General")
        {
            dto.General.Add(value);
        }
        else if (category == "Accessibility")
        {
            dto.Accessibility.Add(value);
        }
        else if (category == "Languages")
        {
            dto.Languages.Add(value);
        }
        else if (category == "Parking")
        {
            dto.Parking.Add(value);
        }
        else if (category == "ReceptionServices")
        {
            dto.ReceptionServices.Add(value);
        }
        else if (category == "CleaningServices")
        {
            dto.CleaningServices.Add(value);
        }
        else if (category == "EntertainmentAndFamily")
        {
            dto.EntertainmentAndFamily.Add(value);
        }
        else if (category == "SafetyAndSecurity")
        {
            dto.SafetyAndSecurity.Add(value);
        }
    }
}