using Booking.Api.Requests.Account;
using Booking.Application.DTOs.Auth;
using Booking.Application.Interfaces.Services;
using Booking.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Booking.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IBookingService _bookingService;
    private readonly IImageService _imageService;

    public AccountController(
        IUserService userService,
        IBookingService bookingService,
        IImageService imageService)
    {
        _userService = userService;
        _bookingService = bookingService;
        _imageService = imageService;
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
      [FromForm] AccountUpdateRequest dto,
      CancellationToken cancellationToken)
    {
        try
        {
            string? avatarUrl = null;

            if (dto.Image != null)
            {
                await using var stream = dto.Image.OpenReadStream();

                avatarUrl = await _imageService.SaveUserAvatarAsync(
                    stream,
                    dto.Image.FileName,
                    cancellationToken);
            }

            var updateDto = new UpdateUserDto
            {
                Name = dto.Name,
                Phone = dto.Phone,
                Country = dto.Country,
                City = dto.City,
                TravelPurpose = dto.TravelPurpose,
                TravelingWithPet = dto.TravelingWithPet
            };

            await _userService.UpdateAsync(
                GetUserId(),
                updateDto,
                avatarUrl,
                cancellationToken);

            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    //[HttpPut]
    //public async Task<IActionResult> Update(
    //    UpdateUserDto dto,
    //    CancellationToken cancellationToken)
    //{
    //    try
    //    {
    //        await _userService.UpdateAsync(
    //            GetUserId(),
    //            dto,
    //            cancellationToken);

    //        return NoContent();
    //    }
    //    catch (Exception ex)
    //    {
    //        return BadRequest(new { message = ex.Message });
    //    }
    //}

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
