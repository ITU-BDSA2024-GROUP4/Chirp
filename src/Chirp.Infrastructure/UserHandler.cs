#nullable disable
using System.Security.Claims;

namespace Chirp.Infrastructure;

public static class UserHandler
{
    public static string FindEmail(ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.Email)?.Value;
    }
    public static string FindName(ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.Name)?.Value;
    }
}