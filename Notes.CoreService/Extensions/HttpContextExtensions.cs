using System.Security.Claims;
using Notes.CoreService.Exceptions;

namespace Notes.CoreService.Extensions;

public static class HttpContextExtensions
{
    public static Guid GetUserId(this HttpContext context)
    {
        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw DomainException.Unauthorized();
        return Guid.Parse(userId);
    }
}