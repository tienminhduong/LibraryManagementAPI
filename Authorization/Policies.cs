using Microsoft.AspNetCore.Authorization;

namespace LibraryManagementAPI.Authorization;

/// <summary>
/// Authorization policies for the application
/// </summary>
public static class Policies
{
    public const string AdminOnly = "AdminOnly";
    public const string StaffOrAdmin = "StaffOrAdmin";
    public const string MemberOnly = "MemberOnly";
    public const string Authenticated = "Authenticated";
}

/// <summary>
/// Role names matching the Role enum
/// </summary>
public static class Roles
{
    public const string Admin = "Admin";
    public const string Staff = "Staff";
    public const string Member = "Member";
    public const string Guest = "Guest";
}
