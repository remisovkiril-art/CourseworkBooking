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
            VerificationRequiredDto result =
                await _authService.RegisterAsync(
                    dto,
                    cancellationToken);

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
            VerificationRequiredDto result =
                await _authService.LoginAsync(
                    dto,
                    cancellationToken);

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
            AuthResponseDto result =
                await _authService.VerifyAsync(
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

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(
        ForgotPasswordDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            VerificationRequiredDto result =
                await _authService.ForgotPasswordAsync(
                    dto,
                    cancellationToken);

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

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(
        ResetPasswordDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            await _authService.ResetPasswordAsync(
                dto,
                cancellationToken);

            return Ok(new
            {
                message = "Password has been changed successfully."
            });
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
        AuthenticationProperties properties =
            new AuthenticationProperties
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
        AuthenticateResult result =
            await HttpContext.AuthenticateAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

        if (!result.Succeeded ||
            result.Principal == null)
        {
            return BadRequest(new
            {
                message = "Google authentication failed",
                failure = result.Failure?.Message
            });
        }

        string? email = result.Principal.FindFirstValue(
            ClaimTypes.Email);

        string? name = result.Principal.FindFirstValue(
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

        AuthResponseDto authResult =
            await _authService.GoogleLoginAsync(
                email,
                name,
                cancellationToken);

        SetRefreshTokenCookie(authResult.RefreshToken);

        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);

        string frontendUrl =
            "http://localhost:5173/google-callback";

        string url =
            $"{frontendUrl}" +
            $"#accessToken={Uri.EscapeDataString(authResult.AccessToken)}" +
            $"&refreshToken={Uri.EscapeDataString(authResult.RefreshToken)}" +
            $"&email={Uri.EscapeDataString(authResult.Email)}" +
            $"&role={authResult.Role}";

        return Redirect(url);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        Response.Cookies.Delete("refreshToken");

        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);

        return Ok(new
        {
            message = "Logout successful"
        });
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

