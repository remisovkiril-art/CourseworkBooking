using System.Security.Claims;
using Booking.Api.Requests.Account;
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
        UpdateUserDto? result =
            await _userService.GetAsync(
                GetUserId(),
                cancellationToken);

        if (result == null)
        {
            return NotFound();
        }

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
                await using Stream stream =
                    dto.Image.OpenReadStream();

                avatarUrl =
                    await _imageService.SaveUserAvatarAsync(
                        stream,
                        dto.Image.FileName,
                        cancellationToken);
            }

            UpdateUserDto updateDto =
                new UpdateUserDto
                {
                    Name = dto.Name,
                    Email = dto.Email,
                    Phone = dto.Phone,
                    Country = dto.Country,
                    City = dto.City,
                    PreferredCurrency = dto.PreferredCurrency,
                    TravelPurpose = dto.TravelPurpose,
                    TravelingWithPet = dto.TravelingWithPet,
                    Gender = dto.Gender,
                    DateOfBirth = dto.DateOfBirth
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
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    [HttpGet("bookings")]
    public async Task<IActionResult> GetBookings(
        CancellationToken cancellationToken)
    {
        List<Booking.Application.DTOs.Bookings.BookingDto> result =
            await _bookingService.GetMyBookingsAsync(
                GetUserId(),
                cancellationToken);

        return Ok(result);
    }

    [HttpGet("travel-preferences")]
    public async Task<IActionResult> GetTravelPreferences(
        CancellationToken cancellationToken)
    {
        TravelPreferencesDto result =
            await _userService.GetTravelPreferencesAsync(
                GetUserId(),
                cancellationToken);

        return Ok(result);
    }

    [HttpPut("travel-preferences")]
    public async Task<IActionResult> SaveTravelPreferences(
        TravelPreferencesDto dto,
        CancellationToken cancellationToken)
    {
        await _userService.SaveTravelPreferencesAsync(
            GetUserId(),
            dto,
            cancellationToken);

        return Ok(new
        {
            message = "Travel preferences saved."
        });
    }

    [HttpGet("newsletter")]
    public async Task<IActionResult> GetNewsletter(
        CancellationToken cancellationToken)
    {
        NewsletterDto? result =
            await _userService.GetNewsletterAsync(
                GetUserId(),
                cancellationToken);

        return Ok(result);
    }

    [HttpPut("newsletter")]
    public async Task<IActionResult> SaveNewsletter(
        NewsletterDto dto,
        CancellationToken cancellationToken)
    {
        await _userService.SaveNewsletterAsync(
            GetUserId(),
            dto,
            cancellationToken);

        return Ok(new
        {
            message = "Newsletter settings saved."
        });
    }

    private Guid GetUserId()
    {
        string? value =
            User.FindFirstValue(
                ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(value))
        {
            throw new Exception("User is not authorized");
        }

        return Guid.Parse(value);
    }
}