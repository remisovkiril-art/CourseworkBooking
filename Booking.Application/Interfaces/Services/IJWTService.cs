using Booking.Domain.Entities;

namespace Booking.Application.Interfaces.Services;

public interface IJwtService
{
    string GenerateAccessToken(User user);

    string GenerateRefreshToken();
}