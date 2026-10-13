using System.Security.Claims;
using Booking.Application.DTOs.Hotels;
using Booking.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class FavoriteController : ControllerBase
{
    private readonly IFavoriteService _favoriteService;
    private readonly IHotelService _hotelService;

    public FavoriteController(
        IFavoriteService favoriteService,
        IHotelService hotelService)
    {
        _favoriteService = favoriteService;
        _hotelService = hotelService;
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        List<Guid> hotelIds = await _favoriteService.GetHotelIdsAsync(
            GetUserId(), cancellationToken);

        List<HotelDto> hotels = new();
        foreach (Guid hotelId in hotelIds)
        {
            var hotel = await _hotelService.GetByIdAsync(hotelId, cancellationToken);
            if (hotel != null)
            {
                hotels.Add(hotel);
            }
        }

        return Ok(hotels);
    }

    [HttpGet("ids")]
    public async Task<IActionResult> GetIds(CancellationToken cancellationToken)
    {
        List<Guid> hotelIds = await _favoriteService.GetHotelIdsAsync(
            GetUserId(), cancellationToken);

        return Ok(hotelIds);
    }

    [HttpPut("{hotelId:guid}")]
    public async Task<IActionResult> Add(
        Guid hotelId,
        CancellationToken cancellationToken)
    {
        try
        {
            await _favoriteService.AddAsync(
                GetUserId(), hotelId, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpDelete("{hotelId:guid}")]
    public async Task<IActionResult> Remove(
        Guid hotelId,
        CancellationToken cancellationToken)
    {
        await _favoriteService.RemoveAsync(
            GetUserId(), hotelId, cancellationToken);
        return NoContent();
    }

    private Guid GetUserId()
    {
        string? value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new UnauthorizedAccessException("User is not authorized.");
        }

        return Guid.Parse(value);
    }
}

