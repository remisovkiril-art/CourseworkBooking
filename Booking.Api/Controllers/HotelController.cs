using Booking.Application.DTOs.Hotels;
using Booking.Application.Interfaces.Repository;
using Booking.Application.Interfaces.Services;
using Booking.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class HotelController : ControllerBase
{
    private readonly IHotelService _hotelService;
    private readonly IHotelRepository _hotelRepository;
    private readonly IImageService _imageService;

    public HotelController(
        IHotelService hotelService,
        IHotelRepository hotelRepository,
        IImageService imageService)
    {
        _hotelService = hotelService;
        _hotelRepository = hotelRepository;
        _imageService = imageService;
    }

    [HttpGet]
    public async Task<IActionResult> GetHotels(
        string? search,
        int adults = 0,
        int children = 0,
        int rooms = 0,
        DateTime? checkIn = null,
        DateTime? checkOut = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _hotelService.GetAllAsync(
            search,
            adults,
            children,
            rooms,
            checkIn,
            checkOut,
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetHotel(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _hotelService.GetByIdAsync(
            id,
            cancellationToken);

        return result == null ? NotFound() : Ok(result);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateHotel(
        HotelCreateDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _hotelService.CreateAsync(
                dto,
                cancellationToken);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize]
    [HttpPost("{id:guid}/image")]
    public async Task<IActionResult> UploadImage(
        Guid id,
        IFormFile file,
        CancellationToken cancellationToken)
    {
        var hotel = await _hotelRepository.GetByIdAsync(
            id,
            cancellationToken);

        if (hotel == null)
            return NotFound(new { message = "Hotel not found" });

        if (file == null || file.Length == 0)
            return BadRequest(new { message = "Image is empty" });

        var allowedTypes = new[]
        {
            "image/jpeg",
            "image/png",
            "image/webp"
        };

        if (!allowedTypes.Contains(file.ContentType))
            return BadRequest(new { message = "Only JPG, PNG and WEBP files are allowed" });

        await using var stream = file.OpenReadStream();

        var url = await _imageService.SaveHotelImageAsync(
            stream,
            file.FileName,
            cancellationToken);

        var image = new HotelImage
        {
            Id = Guid.NewGuid(),
            HotelId = id,
            ImageUrl = url
        };

        await _hotelRepository.AddImageAsync(
            image,
            cancellationToken);

        return Ok(new { imageUrl = url });
    }
}
