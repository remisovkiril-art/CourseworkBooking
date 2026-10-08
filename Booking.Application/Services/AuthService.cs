using AutoMapper;
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
    private readonly AdminSettings _adminSettings;
    private readonly IMapper _mapper;

    public AuthService(
        IUserRepository userRepository,
        IJwtService jwtService,
        IEmailService emailService,
        JwtSettings jwtSettings,
        AdminSettings adminSettings,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
        _emailService = emailService;
        _jwtSettings = jwtSettings;
        _adminSettings = adminSettings;
        _mapper = mapper;
    }

    public async Task<VerificationRequiredDto> RegisterAsync(
        RegisterDto dto,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(dto.Email))
        {
            throw new Exception("Email is required");
        }

        if (string.IsNullOrWhiteSpace(dto.Password))
        {
            throw new Exception("Password is required");
        }

        if (dto.Password.Length < 6)
        {
            throw new Exception("Password must contain at least 6 characters");
        }

        User? existingUser = await _userRepository.GetByEmailAsync(
            dto.Email,
            cancellationToken);

        if (existingUser != null)
        {
            throw new Exception("User with this email already exists");
        }

        string verificationCode = GenerateCode();

        int role = 0;

        if (!string.IsNullOrWhiteSpace(_adminSettings.FirstAdminEmail) &&
            dto.Email.Equals(
                _adminSettings.FirstAdminEmail,
                StringComparison.OrdinalIgnoreCase))
        {
            role = 1;
        }

        User user = new User
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Email = dto.Email.Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            VerificationCode = verificationCode,
            VerificationCodeExpiresAt = DateTime.UtcNow.AddMinutes(10),
            VerificationPurpose = "Registration",
            IsVerified = false,
            Role = role
        };

        await _userRepository.AddAsync(
            user,
            cancellationToken);

        await _emailService.SendVerificationCodeAsync(
            user.Email,
            verificationCode,
            cancellationToken);

        return new VerificationRequiredDto
        {
            Email = user.Email,
            Purpose = "Registration",
            Message = "Verification code has been sent to your email."
        };
    }

    public async Task<VerificationRequiredDto> LoginAsync(
        LoginDto dto,
        CancellationToken cancellationToken)
    {
        User? user = await _userRepository.GetByEmailAsync(
            dto.Email,
            cancellationToken);

        if (user == null)
        {
            throw new Exception("Invalid email or password");
        }

        bool validPassword = BCrypt.Net.BCrypt.Verify(
            dto.Password,
            user.PasswordHash);

        if (!validPassword)
        {
            throw new Exception("Invalid email or password");
        }

        string verificationCode = GenerateCode();

        user.VerificationCode = verificationCode;
        user.VerificationCodeExpiresAt = DateTime.UtcNow.AddMinutes(10);
        user.VerificationPurpose = "Login";

        await _userRepository.UpdateAsync(
            user,
            cancellationToken);

        await _emailService.SendVerificationCodeAsync(
            user.Email,
            verificationCode,
            cancellationToken);

        return new VerificationRequiredDto
        {
            Email = user.Email,
            Purpose = "Login",
            Message = "Login verification code has been sent to your email."
        };
    }

    public async Task<AuthResponseDto> VerifyAsync(
        VerifyCodeDto dto,
        CancellationToken cancellationToken)
    {
        User? user = await _userRepository.GetByEmailAsync(
            dto.Email,
            cancellationToken);

        if (user == null)
        {
            throw new Exception("User not found");
        }

        if (!string.Equals(
                user.VerificationCode,
                dto.Code,
                StringComparison.Ordinal))
        {
            throw new Exception("Invalid verification code");
        }

        if (user.VerificationCodeExpiresAt == null ||
            user.VerificationCodeExpiresAt < DateTime.UtcNow)
        {
            throw new Exception("Verification code has expired");
        }

        if (!string.Equals(
                user.VerificationPurpose,
                dto.Purpose,
                StringComparison.OrdinalIgnoreCase))
        {
            throw new Exception("Invalid verification purpose");
        }

        if (dto.Purpose.Equals(
                "Registration",
                StringComparison.OrdinalIgnoreCase))
        {
            user.IsVerified = true;
        }

        if (dto.Purpose.Equals(
                "Login",
                StringComparison.OrdinalIgnoreCase))
        {
            user.IsVerified = true;
        }

        user.VerificationCode = string.Empty;
        user.VerificationCodeExpiresAt = null;
        user.VerificationPurpose = string.Empty;

        user.RefreshToken = _jwtService.GenerateRefreshToken();
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(
            _jwtSettings.RefreshTokenDays);

        await _userRepository.UpdateAsync(
            user,
            cancellationToken);

        return CreateResponse(user);
    }

    public async Task<AuthResponseDto> GoogleLoginAsync(
        string email,
        string name,
        CancellationToken cancellationToken)
    {
        User? user = await _userRepository.GetByEmailAsync(
            email,
            cancellationToken);

        if (user == null)
        {
            user = new User
            {
                Id = Guid.NewGuid(),
                Name = name,
                Email = email,
                PasswordHash = string.Empty,
                VerificationCode = string.Empty,
                VerificationCodeExpiresAt = null,
                VerificationPurpose = string.Empty,
                IsVerified = true,
                Role = 0
            };

            user.RefreshToken = _jwtService.GenerateRefreshToken();
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(
                _jwtSettings.RefreshTokenDays);

            await _userRepository.AddAsync(
                user,
                cancellationToken);
        }
        else
        {
            if (string.IsNullOrWhiteSpace(user.Name))
            {
                user.Name = name;
            }

            user.IsVerified = true;

            user.RefreshToken = _jwtService.GenerateRefreshToken();
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(
                _jwtSettings.RefreshTokenDays);

            await _userRepository.UpdateAsync(
                user,
                cancellationToken);
        }

        return CreateResponse(user);
    }

    public async Task<VerificationRequiredDto> ForgotPasswordAsync(
        ForgotPasswordDto dto,
        CancellationToken cancellationToken)
    {
        User? user = await _userRepository.GetByEmailAsync(
            dto.Email,
            cancellationToken);

        if (user == null)
        {
            throw new Exception("User with this email was not found");
        }

        string code = GenerateCode();

        user.VerificationCode = code;
        user.VerificationCodeExpiresAt = DateTime.UtcNow.AddMinutes(10);
        user.VerificationPurpose = "PasswordReset";

        await _userRepository.UpdateAsync(
            user,
            cancellationToken);

        await _emailService.SendVerificationCodeAsync(
            user.Email,
            code,
            cancellationToken);

        return new VerificationRequiredDto
        {
            Email = user.Email,
            Purpose = "PasswordReset",
            Message = "Password reset code has been sent to your email."
        };
    }

    public async Task ResetPasswordAsync(
        ResetPasswordDto dto,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(dto.NewPassword))
        {
            throw new Exception("New password is required");
        }

        if (dto.NewPassword.Length < 6)
        {
            throw new Exception(
                "Password must contain at least 6 characters");
        }

        User? user = await _userRepository.GetByEmailAsync(
            dto.Email,
            cancellationToken);

        if (user == null)
        {
            throw new Exception("User not found");
        }

        if (!string.Equals(
                user.VerificationCode,
                dto.Code,
                StringComparison.Ordinal))
        {
            throw new Exception("Invalid verification code");
        }

        if (user.VerificationCodeExpiresAt == null ||
            user.VerificationCodeExpiresAt < DateTime.UtcNow)
        {
            throw new Exception("Verification code has expired");
        }

        if (!string.Equals(
                user.VerificationPurpose,
                "PasswordReset",
                StringComparison.OrdinalIgnoreCase))
        {
            throw new Exception("Invalid verification purpose");
        }

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(
            dto.NewPassword);

        user.VerificationCode = string.Empty;
        user.VerificationCodeExpiresAt = null;
        user.VerificationPurpose = string.Empty;

        await _userRepository.UpdateAsync(
            user,
            cancellationToken);
    }

    private AuthResponseDto CreateResponse(User user)
    {
        AuthResponseDto response = _mapper.Map<AuthResponseDto>(user);

        response.AccessToken = _jwtService.GenerateAccessToken(user);

        response.AccessTokenExpires = DateTime.UtcNow.AddMinutes(
            _jwtSettings.AccessTokenMinutes);

        response.RefreshToken = user.RefreshToken;

        response.Role = user.Role;

        return response;
    }

    private static string GenerateCode()
    {
        return Random.Shared.Next(100000, 1000000).ToString();
    }
}