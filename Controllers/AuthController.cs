using LibraryManagementAPI.Interfaces.IServices;
using LibraryManagementAPI.Models.Account;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementAPI.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController(
        IAccountService accountService,
        IResetPasswordService resetPasswordService
    ) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CreateAccountDto createAccountDto)
        {
            if(ModelState.IsValid == false)
            {
                throw new ArgumentException("Invalid data.");
            }
            var response = await accountService.Register(createAccountDto);
            if (response.isSuccess == false)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDataDto loginDto)
        {
            if(ModelState.IsValid == false)
            {
                throw new ArgumentException("Invalid data.");
            }
            var response =  await accountService.Login(loginDto.userName, loginDto.password);
            if (response.isSuccess == false)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("request-password-reset")]
        public IActionResult RequestPasswordReset([FromBody] PasswordResetRequestDto requestDto)
        {
            resetPasswordService.SendResetPasswordEmailAsync(requestDto.Email);
            return Ok(new { Message = "If the email exists, a reset token has been sent." });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] PasswordResetDto resetDto)
        {
            var result = await resetPasswordService.ResetPasswordAsync(resetDto.Email, resetDto.Token, resetDto.NewPassword);
            
            if (!result)
                return BadRequest(new { Message = "Invalid token or email." });
            
            return Ok(new { Message = "Password has been reset successfully." });
        }
    }
}
