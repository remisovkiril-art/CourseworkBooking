using Booking.Application.DTOs.Auth;
using Booking.Application.Interfaces.Repository;
using Booking.Application.Interfaces.Services;

namespace Booking.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UpdateUserDto?> GetAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(
            userId,
            cancellationToken);

        if (user == null)
        {
            return null;
        }

        return new UpdateUserDto
        {
            Name = user.Name,
            Phone = user.Phone,
            Country = user.Country,
            City = user.City,
            TravelPurpose = user.TravelPurpose,
            TravelingWithPet = user.TravelingWithPet
        };
    }

    public async Task UpdateAsync(
        Guid userId,
        UpdateUserDto dto,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(
            userId,
            cancellationToken);

        if (user == null)
        {
            throw new Exception("User not found");
        }

        user.Name = dto.Name;
        user.Phone = dto.Phone;
        user.Country = dto.Country;
        user.City = dto.City;
        user.TravelPurpose = dto.TravelPurpose;
        user.TravelingWithPet = dto.TravelingWithPet;

        await _userRepository.UpdateAsync(
            user,
            cancellationToken);
    }
}
