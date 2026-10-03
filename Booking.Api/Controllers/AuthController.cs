using System.Security.Claims;
using Booking.Application.DTOs.Auth;
using Booking.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
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
            return BadRequest(new
            {
                message = ex.Message
            });
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
            return BadRequest(new
            {
                message = ex.Message
            });
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
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    [HttpGet("login-google")]
    public IActionResult LoginGoogle()
    {
        var properties = new AuthenticationProperties
        {
            RedirectUri = Url.Action(
                nameof(ExternalResponse))
        };

        return Challenge(
            properties,
            GoogleDefaults.AuthenticationScheme);
    }

    [HttpGet("external-response")]
    public async Task<IActionResult> ExternalResponse(
        CancellationToken cancellationToken)
    {
        var result = await HttpContext.AuthenticateAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);

        if (!result.Succeeded || result.Principal == null)
        {
            return BadRequest(new
            {
                message = "Google authentication failed",
                failure = result.Failure?.Message,
                details = "Open /api/v1/Auth/login-google first. Do not open /api/v1/Auth/external-response directly."
            });
        }

        var email = result.Principal.FindFirstValue(
            ClaimTypes.Email);

        var name = result.Principal.FindFirstValue(
            ClaimTypes.Name);

        if (string.IsNullOrWhiteSpace(email))
        {
            return BadRequest(new
            {
                message = "Google email was not received"
            });
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            name = email;
        }

        var authResult = await _authService.GoogleLoginAsync(
            email,
            name,
            cancellationToken);

        SetRefreshTokenCookie(authResult.RefreshToken);

        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);

        var frontendUrl =
            "http://localhost:5173/google-callback";

        var url =
            $"{frontendUrl}" +
            $"#accessToken={Uri.EscapeDataString(authResult.AccessToken)}" +
            $"&refreshToken={Uri.EscapeDataString(authResult.RefreshToken)}" +
            $"&email={Uri.EscapeDataString(authResult.Email)}";

        return Redirect(url);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);

        return Ok("Logout successful");
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

