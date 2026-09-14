using Booking.Application.DTOs.Hotels;
using Booking.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class RoomController : ControllerBase
{
    private readonly IRoomService _roomService;

    public RoomController(IRoomService roomService)
    {
        _roomService = roomService;
    }

    [HttpGet("hotel/{hotelId:guid}")]
    public async Task<IActionResult> GetByHotel(
        Guid hotelId,
        CancellationToken cancellationToken)
    {
        var result = await _roomService.GetByHotelIdAsync(
            hotelId,
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _roomService.GetByIdAsync(
            id,
            cancellationToken);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [Authorize]
    [HttpPost("hotel/{hotelId:guid}")]
    public async Task<IActionResult> Create(
        Guid hotelId,
        RoomCreateDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _roomService.CreateAsync(
                hotelId,
                dto,
                cancellationToken);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
