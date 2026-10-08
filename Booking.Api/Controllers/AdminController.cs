using System.Security.Claims;
using Booking.Application.DTOs.Auth;
using Booking.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(
        IAdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpPost("first-admin")]
    public async Task<IActionResult> SetFirstAdmin(
        CancellationToken cancellationToken)
    {
        try
        {
            await _adminService.SetFirstAdminAsync(
                GetUserId(),
                cancellationToken);

            return Ok(new
            {
                message = "Admin role assigned."
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    [HttpPost("moderators")]
    [Authorize(Roles = "1")]
    public async Task<IActionResult> CreateModerator(
        CreateModeratorDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            UserReadDto result =
                await _adminService.CreateModeratorAsync(
                    GetUserId(),
                    dto,
                    cancellationToken);

            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    [HttpDelete("reviews/{reviewId:guid}")]
    [Authorize(Roles = "1,2")]
    public async Task<IActionResult> DeleteReview(
        Guid reviewId,
        CancellationToken cancellationToken)
    {
        try
        {
            await _adminService.DeleteReviewAsync(
                GetUserId(),
                reviewId,
                cancellationToken);

            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
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
        string? value =
            User.FindFirstValue(
                ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new Exception(
                "User is not authorized.");
        }

        return Guid.Parse(value);
    }
}