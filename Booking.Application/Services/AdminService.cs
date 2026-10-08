using Booking.Application.DTOs.Auth;
using Booking.Application.Interfaces.Repository;
using Booking.Application.Interfaces.Services;
using Booking.Application.Settings;
using Booking.Domain.Entities;

namespace Booking.Application.Services;

public class AdminService : IAdminService
{
    private readonly IUserRepository _userRepository;
    private readonly IReviewRepository _reviewRepository;
    private readonly AdminSettings _adminSettings;

    public AdminService(
        IUserRepository userRepository,
        IReviewRepository reviewRepository,
        AdminSettings adminSettings)
    {
        _userRepository = userRepository;
        _reviewRepository = reviewRepository;
        _adminSettings = adminSettings;
    }

    public async Task SetFirstAdminAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        User? user =
            await _userRepository.GetByIdAsync(
                userId,
                cancellationToken);

        if (user == null)
        {
            throw new Exception("User not found.");
        }

        if (!string.Equals(
                user.Email,
                _adminSettings.FirstAdminEmail,
                StringComparison.OrdinalIgnoreCase))
        {
            throw new UnauthorizedAccessException(
                "Only configured first admin can receive admin role.");
        }

        user.Role = 1;

        await _userRepository.UpdateAsync(
            user,
            cancellationToken);
    }

    public async Task<UserReadDto> CreateModeratorAsync(
        Guid adminUserId,
        CreateModeratorDto dto,
        CancellationToken cancellationToken)
    {
        User? admin =
            await _userRepository.GetByIdAsync(
                adminUserId,
                cancellationToken);

        if (admin == null || admin.Role != 1)
        {
            throw new UnauthorizedAccessException(
                "Only admin can create moderator.");
        }

        if (string.IsNullOrWhiteSpace(dto.Email))
        {
            throw new Exception("Email is required.");
        }

        if (string.IsNullOrWhiteSpace(dto.Password))
        {
            throw new Exception("Password is required.");
        }

        User? existingUser =
            await _userRepository.GetByEmailAsync(
                dto.Email,
                cancellationToken);

        if (existingUser != null)
        {
            throw new Exception(
                "User with this email already exists.");
        }

        User moderator = new User
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(
                dto.Password),
            Role = 2,
            IsVerified = true,
            VerificationCode = string.Empty,
            VerificationPurpose = string.Empty,
            VerificationCodeExpiresAt = null,
            RefreshToken = string.Empty,
            RefreshTokenExpiryTime = DateTime.UtcNow
        };

        await _userRepository.AddAsync(
            moderator,
            cancellationToken);

        return new UserReadDto
        {
            Id = moderator.Id,
            Email = moderator.Email,
            Name = moderator.Name
        };
    }

    public async Task DeleteReviewAsync(
        Guid userId,
        Guid reviewId,
        CancellationToken cancellationToken)
    {
        User? user =
            await _userRepository.GetByIdAsync(
                userId,
                cancellationToken);

        if (user == null ||
            (user.Role != 1 && user.Role != 2))
        {
            throw new UnauthorizedAccessException(
                "Only admin or moderator can delete reviews.");
        }

        await _reviewRepository.DeleteAsync(
            reviewId,
            cancellationToken);
    }
}