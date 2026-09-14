using Booking.Application.DTOs.Auth;
using Booking.Application.Interfaces.Repository;
using Booking.Application.Interfaces.Services;
using Booking.Application.Settings;
using Booking.Domain.Entities;

namespace Booking.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;
    private readonly IEmailService _emailService;
    private readonly JwtSettings _jwtSettings;

    public AuthService(
        IUserRepository userRepository,
        IJwtService jwtService,
        IEmailService emailService,
        JwtSettings jwtSettings)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
        _emailService = emailService;
        _jwtSettings = jwtSettings;
    }

    public async Task<AuthResponseDto> RegisterAsync(
        RegisterDto dto,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new Exception("Name is required");

        if (string.IsNullOrWhiteSpace(dto.Email))
            throw new Exception("Email is required");

        if (string.IsNullOrWhiteSpace(dto.Password))
            throw new Exception("Password is required");

        var existingUser = await _userRepository.GetByEmailAsync(
            dto.Email,
            cancellationToken);

        if (existingUser != null)
            throw new Exception("User with this email already exists");

        var verificationCode = Random.Shared.Next(100000, 1000000).ToString();

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Email = dto.Email,
            Phone = dto.Phone,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            VerificationCode = verificationCode,
            IsVerified = false
        };

        user.RefreshToken = _jwtService.GenerateRefreshToken();
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(
            _jwtSettings.RefreshTokenDays);

        await _userRepository.AddAsync(user, cancellationToken);

        await _emailService.SendVerificationCodeAsync(
            user.Email,
            verificationCode,
            cancellationToken);

        return CreateResponse(user, verificationCode);
    }

    public async Task<AuthResponseDto> LoginAsync(
        LoginDto dto,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(
            dto.Email,
            cancellationToken);

        if (user == null)
            throw new Exception("Invalid email or password");

        var validPassword = BCrypt.Net.BCrypt.Verify(
            dto.Password,
            user.PasswordHash);

        if (!validPassword)
            throw new Exception("Invalid email or password");

        user.RefreshToken = _jwtService.GenerateRefreshToken();
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(
            _jwtSettings.RefreshTokenDays);

        await _userRepository.UpdateAsync(user, cancellationToken);

        return CreateResponse(user, user.VerificationCode);
    }

    public async Task<AuthResponseDto> VerifyAsync(
        VerifyCodeDto dto,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(
            dto.Email,
            cancellationToken);

        if (user == null)
            throw new Exception("User not found");

        if (user.VerificationCode != dto.Code)
            throw new Exception("Invalid verification code");

        user.IsVerified = true;
        user.VerificationCode = string.Empty;
        user.RefreshToken = _jwtService.GenerateRefreshToken();
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(
            _jwtSettings.RefreshTokenDays);

        await _userRepository.UpdateAsync(user, cancellationToken);

        return CreateResponse(user, string.Empty);
    }

    private AuthResponseDto CreateResponse(User user, string verificationCode)
    {
        return new AuthResponseDto
        {
            AccessToken = _jwtService.GenerateAccessToken(user),
            AccessTokenExpires = DateTime.UtcNow.AddMinutes(
                _jwtSettings.AccessTokenMinutes),
            Name = user.Name,
            Email = user.Email,
            RefreshToken = user.RefreshToken,
            VerificationCode = verificationCode
        };
    }
}