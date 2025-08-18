using System;
using System.Linq;
using System.Security.Claims;

namespace Minicommerce.Shared.Extensions;

public static class ClaimsPrincipleExtensions
{
    public static string GetUserName(this ClaimsPrincipal user)
    {
        var username = user.FindFirst(ClaimTypes.Name)?.Value ?? throw new Exception("Cannot get username from token");
        return username;
    }

    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new Exception("Cannot get user ID from token");
        return Guid.Parse(userIdClaim);
    }

    // Additional helper methods for your ApplicationUser properties
    public static string GetFirstName(this ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.GivenName)?.Value ?? string.Empty;
    }

    public static string GetLastName(this ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.Surname)?.Value ?? string.Empty;
    }

    public static string GetFullName(this ClaimsPrincipal user)
    {
        return user.FindFirst("FullName")?.Value ?? string.Empty;
    }

    public static IEnumerable<string> GetRoles(this ClaimsPrincipal user)
    {
        return user.FindAll(ClaimTypes.Role).Select(c => c.Value);
    }

    public static string GetPrimaryRole(this ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
    }


    public static string GetPosition(this ClaimsPrincipal user)
    {
        return user.FindFirst("Position")?.Value ?? string.Empty;
    }

    public static bool IsActive(this ClaimsPrincipal user)
    {
        var isActiveClaim = user.FindFirst("IsActive")?.Value;
        return bool.TryParse(isActiveClaim, out var isActive) && isActive;
    }


    // Helper method to check if user has a specific role
    public static bool IsInRole(this ClaimsPrincipal user, string role)
    {
        return user.HasClaim(ClaimTypes.Role, role);
    }


    public static bool IsAdmin(this ClaimsPrincipal user)
    {
        return user.IsInRole("Admin");
    }


}