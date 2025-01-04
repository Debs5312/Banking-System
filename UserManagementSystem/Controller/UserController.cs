using Microsoft.AspNetCore.Mvc;
using Models.DTOs;
using UserManagementSystem.Service.IService;

namespace UserManagementSystem.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody]RegistrationDTO registrationDTO, CancellationToken ct)
        {
            try
            {
                if(ct.IsCancellationRequested)
                {
                    ct.ThrowIfCancellationRequested();
                    return NoContent();
                }
                else
                {
                    var registrationStatus = await _userService.RegisterNewUser(registrationDTO);
                    if(registrationStatus == 1) return StatusCode(409, "User alreday exists");
                    else if(registrationStatus == 2) return StatusCode(409, "Adhar card alreday exists");
                    else if(registrationStatus == 3) return Ok();
                    else return BadRequest();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e} thrown with message: {e.Message}");
                return StatusCode(500);
            }
        }
    }
}