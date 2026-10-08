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
    public async Task<ActionResult<HotelSearchResultDto>> GetAll(
        [FromQuery] HotelSearchDto request,
        CancellationToken cancellationToken)
    {
        HotelSearchResultDto result = await _hotelService.SearchAsync(
            request,
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("random")]
    public async Task<ActionResult<List<HotelDto>>> GetRandomHotels(
        [FromQuery] int number = 8,
        CancellationToken cancellationToken = default)
    {
        if (number <= 0)
            return BadRequest("Number must be greater than 0.");

        List<HotelDto>? hotels = await _hotelService.GetRandomAsync(
            number,
            cancellationToken);

        return Ok(hotels);
    }

    [HttpGet("filters")]
    public async Task<ActionResult<HotelFiltersDto>> GetFilters(
        [FromQuery] HotelSearchDto request,
        CancellationToken cancellationToken)
    {
        HotelFiltersDto result = await _hotelService.GetFiltersAsync(
            request,
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetHotel(
        Guid id,
        CancellationToken cancellationToken)
    {
        HotelDto? result = await _hotelService.GetByIdAsync(
            id,
            cancellationToken);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [Authorize(Roles = "1")]
    [HttpPost]
    public async Task<IActionResult> CreateHotel(
        HotelCreateDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            HotelDto result = await _hotelService.CreateAsync(
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

    [Authorize(Roles = "1")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteHotel(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            await _hotelService.DeleteAsync(
                id,
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

    [Authorize(Roles = "1")]
    [HttpPost("{id:guid}/image")]
    public async Task<IActionResult> UploadImage(
        Guid id,
        IFormFile file,
        CancellationToken cancellationToken)
    {
        Hotel? hotel = await _hotelRepository.GetByIdAsync(
            id,
            cancellationToken);

        if (hotel == null)
        {
            return NotFound(new
            {
                message = "Hotel not found"
            });
        }

        if (file == null || file.Length == 0)
        {
            return BadRequest(new
            {
                message = "Image is empty"
            });
        }

        string[] allowedTypes =
        {
            "image/jpeg",
            "image/png",
            "image/webp"
        };

        if (!allowedTypes.Contains(file.ContentType))
        {
            return BadRequest(new
            {
                message = "Only JPG, PNG and WEBP files are allowed"
            });
        }

        await using Stream stream = file.OpenReadStream();

        string url = await _imageService.SaveHotelImageAsync(
            stream,
            file.FileName,
            cancellationToken);

        HotelImage image = new HotelImage
        {
            Id = Guid.NewGuid(),
            HotelId = id,
            ImageUrl = url
        };

        await _hotelRepository.AddImageAsync(
            image,
            cancellationToken);

        return Ok(new
        {
            imageUrl = url
        });
    }

    [HttpGet("favorites")]
    public async Task<IActionResult> GetFavoriteHotels(
    [FromQuery] List<Guid> ids,
    CancellationToken cancellationToken)
    {
        var hotels = await _hotelService.GetFavoritesAsync(ids,cancellationToken);

        if(hotels == null)
        {
            return NotFound();
        }

        return Ok(hotels);
    }
}
