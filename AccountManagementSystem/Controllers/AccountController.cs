using AccountManagementSystem.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace AccountManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }


        [HttpGet("AllAccounts")]
        public async Task<IActionResult> Get()
        {
            var accounts = await _accountService.GetAccounts();

            if(accounts.Any()) return Ok(accounts);

            return NotFound();
        }
    }
}