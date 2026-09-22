using AutoMapper;
using Booking.Application.DTOs.Auth;
using Booking.Application.Interfaces.Repository;
using Booking.Application.Interfaces.Services;

namespace Booking.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserService(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
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

        return _mapper.Map<UpdateUserDto>(user);

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

        _mapper.Map(dto, user);

        await _userRepository.UpdateAsync(
            user,
            cancellationToken);
    }
}