using Microsoft.AspNetCore.Mvc;

namespace AccountManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        [HttpGet("GetAllAccounts")]
        public async Task<IActionResult> Get()
        {
            return Ok("Accounts");
        }
    }
}