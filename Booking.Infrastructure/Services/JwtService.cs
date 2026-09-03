using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

using Booking.Application.Interfaces.Services;
using Booking.Application.Settings;
using Booking.Domain.Entities;

using Microsoft.IdentityModel.Tokens;

namespace Booking.Infrastructure.Services;

public class JwtService : IJwtService
{
    private readonly JwtSettings _settings;

    public JwtService(JwtSettings settings)
    {
        _settings = settings;
    }

    public string GenerateAccessToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(
                JwtRegisteredClaimNames.Sub,
                user.Id.ToString()),

            new Claim(
                JwtRegisteredClaimNames.Email,
                user.Email),

            new Claim(
                ClaimTypes.Name,
                user.Name)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_settings.Key));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                _settings.AccessTokenMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        return Convert.ToBase64String(
            RandomNumberGenerator.GetBytes(64));
    }
}