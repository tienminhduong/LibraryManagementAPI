namespace LibraryManagementAPI.Models.Utility
{
    public static class Utility
    {
        static public string GetClaim(HttpContext httpContext, string claimType)
        {
            var claim = httpContext.User.Claims.FirstOrDefault(c => c.Type == claimType);
            return claim?.Value ?? string.Empty;
        }
    }
}
