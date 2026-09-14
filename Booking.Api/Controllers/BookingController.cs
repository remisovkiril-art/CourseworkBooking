using System.Security.Claims;
using Booking.Application.DTOs.Bookings;
using Booking.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class BookingController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateBookingDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserId();

            var result = await _bookingService.CreateAsync(
                userId,
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

    private Guid GetUserId()
    {
        var value = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(value))
        {
            throw new Exception(
                "User is not authorized");
        }

        return Guid.Parse(value);
    }
}

