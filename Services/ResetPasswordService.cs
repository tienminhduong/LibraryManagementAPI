using LibraryManagementAPI.Interfaces.IServices;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace LibraryManagementAPI.Services;

public class ResetPasswordService(IServiceScopeFactory scopeFactory) : IResetPasswordService
{
    private readonly string _senderEmail = Environment.GetEnvironmentVariable("MAIL_ADDRESS") ?? "";
    private readonly string _appMailPassword = Environment.GetEnvironmentVariable("MAIL_APP_PASSWORD") ?? "";
    
    private const string SenderName = "Library Management";

    private readonly Dictionary<string, ResetTokenInfo> _resetTokens = new();

    public void SendResetPasswordEmailAsync(string email)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(SenderName, _senderEmail));
        message.To.Add(new MailboxAddress("", email));
        message.Subject = "Password Reset Request";
        
        var resetToken = GenerateResetToken();
        _resetTokens[email] = new ResetTokenInfo()
        {
            Token = resetToken,
            Expiry = DateTime.UtcNow.AddMinutes(10)
        };
        message.Body = new TextPart("html")
        {
            Text = $"<p>Your password reset token is: <strong>{resetToken}</strong></p>" +
                   "<p>Please use this token to reset your password. This token is valid for 15 minutes.</p>"
        };
        
        using var client = new SmtpClient();
        client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
        client.Authenticate(_senderEmail, _appMailPassword);
        client.Send(message);
        client.Disconnect(true);
    }

    public async Task<bool> ResetPasswordAsync(string userEmail, int token, string newPassword)
    {
        if (!_resetTokens.TryGetValue(userEmail, out var tokenInfo))
            return false;

        if (tokenInfo.Token != token || tokenInfo.Expiry < DateTime.UtcNow)
            return false;
        
        var accountService = scopeFactory.CreateScope().ServiceProvider.GetRequiredService<IAccountService>();
        var result = await accountService.ResetPassword(userEmail, newPassword);

        if (result.isSuccess)
        {
            _resetTokens.Remove(userEmail);
            return true;
        }
        else {
            throw new Exception(result.errorMessage);
        }
    }
    
    private static int GenerateResetToken()
    {
        var random = new Random();
        return random.Next(100000, 999999);
    }
    
    private class ResetTokenInfo
    {
        public int Token { get; init; }
        public DateTime Expiry { get; init; }
    }
}