using System.Security.Claims;
using Booking.Application.DTOs.Auth;
using Booking.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IBookingService _bookingService;

    public AccountController(
        IUserService userService,
        IBookingService bookingService)
    {
        _userService = userService;
        _bookingService = bookingService;
    }

    [HttpGet]
    public async Task<IActionResult> Get(
        CancellationToken cancellationToken)
    {
        var result = await _userService.GetAsync(
            GetUserId(),
            cancellationToken);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpPut]
    public async Task<IActionResult> Update(
        UpdateUserDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            await _userService.UpdateAsync(
                GetUserId(),
                dto,
                cancellationToken);

            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("bookings")]
    public async Task<IActionResult> GetBookings(
        CancellationToken cancellationToken)
    {
        var result = await _bookingService.GetMyBookingsAsync(
            GetUserId(),
            cancellationToken);

        return Ok(result);
    }

    private Guid GetUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(value))
            throw new Exception("User is not authorized");

        return Guid.Parse(value);
    }
}
