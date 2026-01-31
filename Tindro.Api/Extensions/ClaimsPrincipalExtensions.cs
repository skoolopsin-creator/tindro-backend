using System.Security.Claims;

namespace Tindro.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        if (user == null)
            throw new UnauthorizedAccessException("User is not authenticated");

        var id =
            user.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
            user.FindFirst("id")?.Value ??
            user.FindFirst("sub")?.Value;

        if (string.IsNullOrWhiteSpace(id))
            throw new UnauthorizedAccessException("User ID claim not found");

        if (!Guid.TryParse(id, out var userId))
            throw new UnauthorizedAccessException("Invalid user ID format");

        return userId;
    }
}
