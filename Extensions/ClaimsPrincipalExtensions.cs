namespace LibraryManagementAPI.Extensions;

public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Gets the user ID (Account ID) from JWT claims
    /// </summary>
    public static Guid GetUserId(this System.Security.Claims.ClaimsPrincipal user)
    {
        var userIdClaim = user.FindFirst(Models.Utility.CustomClaims.UserId)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }

    /// <summary>
    /// Gets the user role from JWT claims
    /// </summary>
    public static string GetUserRole(this System.Security.Claims.ClaimsPrincipal user)
    {
        return user.FindFirst(Models.Utility.CustomClaims.Role)?.Value ?? string.Empty;
    }

    /// <summary>
    /// Gets the username from JWT claims
    /// </summary>
    public static string GetUserName(this System.Security.Claims.ClaimsPrincipal user)
    {
        return user.FindFirst(Models.Utility.CustomClaims.Name)?.Value ?? string.Empty;
    }

    /// <summary>
    /// Checks if the user has a specific role
    /// </summary>
    public static bool HasRole(this System.Security.Claims.ClaimsPrincipal user, string role)
    {
        return user.GetUserRole().Equals(role, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Checks if the user is an Admin
    /// </summary>
    public static bool IsAdmin(this System.Security.Claims.ClaimsPrincipal user)
    {
        return user.HasRole("Admin");
    }

    /// <summary>
    /// Checks if the user is Staff or Admin
    /// </summary>
    public static bool IsStaffOrAdmin(this System.Security.Claims.ClaimsPrincipal user)
    {
        var role = user.GetUserRole();
        return role.Equals("Staff", StringComparison.OrdinalIgnoreCase) || 
               role.Equals("Admin", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Checks if the user is a Member
    /// </summary>
    public static bool IsMember(this System.Security.Claims.ClaimsPrincipal user)
    {
        return user.HasRole("Member");
    }
}
