namespace LibraryManagementAPI.Interfaces.IServices;

public interface IResetPasswordService
{
    void SendResetPasswordEmailAsync(string email);
    Task<bool> ResetPasswordAsync(string userEmail, int token, string newPassword);
}