using System.Security.Claims;
using Booking.Application.DTOs.Auth;
using Booking.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class PaymentMethodController : ControllerBase
{
    private readonly IPaymentMethodService _service;

    public PaymentMethodController(IPaymentMethodService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Get(
        CancellationToken cancellationToken)
    {
        return Ok(await _service.GetAsync(
            GetUserId(),
            cancellationToken));
    }

    [HttpPost]
    public async Task<IActionResult> Add(
        AddPaymentMethodDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            await _service.AddAsync(
                GetUserId(),
                dto,
                cancellationToken);

            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    private Guid GetUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? throw new Exception("User is not authorized");

        return Guid.Parse(value);
    }
}
