using System.Security.Claims;
using Booking.Application.DTOs.Reviews;
using Booking.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ReviewController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpGet("hotel/{hotelId:guid}")]
    public async Task<IActionResult> Get(
        Guid hotelId,
        CancellationToken cancellationToken)
    {
        var result =
            await _reviewService.GetByHotelIdAsync(
                hotelId,
                cancellationToken);

        return Ok(result);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(
        CreateReviewDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserId();

            var result =
                await _reviewService.AddAsync(
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
