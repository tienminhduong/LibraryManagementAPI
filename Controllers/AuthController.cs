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
        public async Task<Response<bool>> Register([FromBody] CreateAccountDto createAccountDto)
        {
            if(ModelState.IsValid == false)
            {
                throw new ArgumentException("Invalid data.");
            }
            return await accountService.Register(createAccountDto);
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
    }
}
