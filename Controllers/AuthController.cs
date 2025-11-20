using LibraryManagementAPI.Interfaces.IServices;
using LibraryManagementAPI.Models.Account;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementAPI.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController(IAccountService accountService) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<bool> Register([FromBody] CreateAccountDto createAccountDto)
        {
            if(ModelState.IsValid == false)
            {
                throw new ArgumentException("Invalid data.");
            }
            return await accountService.Register(createAccountDto);
        }
    }
}
