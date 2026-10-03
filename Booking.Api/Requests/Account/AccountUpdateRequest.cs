using Booking.Application.DTOs.Auth;

namespace Booking.Api.Requests.Account;

public class AccountUpdateRequest: UpdateUserDto
{
    public IFormFile? Image { get; set; }
}
