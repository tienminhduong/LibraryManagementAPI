namespace LibraryManagementAPI.Models.Account;

public class PasswordResetDto
{
    public required string Email { get; set; }
    public int Token { get; set; }
    public required string NewPassword { get; set; }
}