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
    private readonly IPdfService _pdfService;
    public BookingController(
        IBookingService bookingService,
        IPdfService pdfService)
    {
        _bookingService = bookingService;
        _pdfService = pdfService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateBookingDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            Guid userId = GetUserId();

            BookingDto result =
                await _bookingService.CreateAsync(
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
    [HttpGet("{bookingId:guid}/pdf")]
    public async Task<IActionResult> GetPdf(
    Guid bookingId,
    CancellationToken cancellationToken)
    {
        byte[] pdf =
            await _pdfService.GenerateBookingPdfAsync(
                bookingId,
                GetUserId(),
                cancellationToken);

        return File(
            pdf,
            "application/pdf",
            $"booking-{bookingId}.pdf");
    }
    private Guid GetUserId()
    {
        string? value =
            User.FindFirstValue(
                ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(value))
        {
            throw new Exception(
                "User is not authorized");
        }

        return Guid.Parse(value);
    }
}
