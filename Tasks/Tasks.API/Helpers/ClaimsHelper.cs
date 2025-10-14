using System.Security.Claims;
using Tasks.Application.Exceptions;

namespace Tasks.API.Helpers;

public static class ClaimsHelper
{
    public static string GetNameIdentifier(ClaimsPrincipal claimsPrincipal)
    {
        var volunteerId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (volunteerId is null)
        {
            throw new UnauthorizedException("User identifier not found in token");
        }
        
        return volunteerId;
    }
}