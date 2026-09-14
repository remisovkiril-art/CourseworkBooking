using Booking.Application.DTOs.Auth;
using Booking.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(
        RegisterDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _authService.RegisterAsync(
                dto,
                cancellationToken);

            SetRefreshTokenCookie(result.RefreshToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        LoginDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _authService.LoginAsync(
                dto,
                cancellationToken);

            SetRefreshTokenCookie(result.RefreshToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("verify")]
    public async Task<IActionResult> Verify(
        VerifyCodeDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _authService.VerifyAsync(
                dto,
                cancellationToken);

            SetRefreshTokenCookie(result.RefreshToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    private void SetRefreshTokenCookie(string token)
    {
        Response.Cookies.Append(
            "refreshToken",
            token,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = Request.IsHttps,
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            });
    }
}


