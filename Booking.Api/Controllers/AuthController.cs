using Booking.Application.DTOs.RegistrationDTOs;
using Booking.Application.Interfaces.Services;

using Microsoft.AspNetCore.Mvc;

namespace Booking.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(
        RegisterDto dto)
    {
        var result =
            await _authService.RegisterAsync(dto);

        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        LoginDto dto)
    {
        var result =
            await _authService.LoginAsync(dto);

        return Ok(result);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        var refreshToken =
            Request.Cookies["refreshToken"];

        if (string.IsNullOrEmpty(refreshToken))
        {
            return Unauthorized();
        }

        var result =
            await _authService.RefreshTokenAsync(
                refreshToken);

        return Ok(result);
    }
}