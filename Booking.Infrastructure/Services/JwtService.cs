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
        List<Claim> claims = new List<Claim>
        {
            new Claim(
                JwtRegisteredClaimNames.Sub,
                user.Id.ToString()),

            new Claim(
                ClaimTypes.NameIdentifier,
                user.Id.ToString()),

            new Claim(
                JwtRegisteredClaimNames.Email,
                user.Email),

            new Claim(
                ClaimTypes.Email,
                user.Email),

            new Claim(
                ClaimTypes.Role,
                user.Role.ToString())
        };

        if (!string.IsNullOrWhiteSpace(user.Name))
        {
            claims.Add(
                new Claim(
                    ClaimTypes.Name,
                    user.Name));
        }

        SymmetricSecurityKey key =
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_settings.Key));

        SigningCredentials credentials =
            new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

        JwtSecurityToken token =
            new JwtSecurityToken(
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