using Booking.Application.DTOs.RegistrationDTOs;
using Booking.Application.Interfaces.Repository;
using Booking.Application.Interfaces.Services;
using Booking.Application.Settings;
using Booking.Domain.Entities;

namespace Booking.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;
    private readonly JwtSettings _jwtSettings;

    public AuthService(
        IUserRepository userRepository,
        IJwtService jwtService,
        JwtSettings jwtSettings)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
        _jwtSettings = jwtSettings;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        var existingUser =
            await _userRepository.GetByEmailAsync(dto.Email);

        if (existingUser != null)
        {
            throw new Exception(
                "User with this email already exists");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Email = dto.Email,
            Phone = dto.Phone,

            PasswordHash =
                BCrypt.Net.BCrypt.HashPassword(
                    dto.Password)
        };

        user.RefreshToken =
            _jwtService.GenerateRefreshToken();

        user.RefreshTokenExpiryTime =
            DateTime.UtcNow.AddDays(
                _jwtSettings.RefreshTokenDays);

        await _userRepository.AddAsync(user);

        return new AuthResponseDto
        {
            AccessToken =
                _jwtService.GenerateAccessToken(user),

            AccessTokenExpires =
                DateTime.UtcNow.AddMinutes(
                    _jwtSettings.AccessTokenMinutes)
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user =
            await _userRepository.GetByEmailAsync(dto.Email);

        if (user == null)
        {
            throw new Exception(
                "Invalid email or password");
        }

        var validPassword =
            BCrypt.Net.BCrypt.Verify(
                dto.Password,
                user.PasswordHash);

        if (!validPassword)
        {
            throw new Exception(
                "Invalid email or password");
        }

        user.RefreshToken =
            _jwtService.GenerateRefreshToken();

        user.RefreshTokenExpiryTime =
            DateTime.UtcNow.AddDays(
                _jwtSettings.RefreshTokenDays);

        await _userRepository.UpdateAsync(user);

        return new AuthResponseDto
        {
            AccessToken =
                _jwtService.GenerateAccessToken(user),

            AccessTokenExpires =
                DateTime.UtcNow.AddMinutes(
                    _jwtSettings.AccessTokenMinutes)
        };
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(
        string refreshToken)
    {
        throw new NotImplementedException();
    }
}