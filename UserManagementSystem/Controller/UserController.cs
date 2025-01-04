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

        [HttpPost("register")]
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

        [HttpGet("login")]
        public async Task<IActionResult> Login([FromBody]LoginDTO loginDTO, CancellationToken ct)
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
                    var loggedInResult = await _userService.LoginUser(loginDTO);
                    if(!loggedInResult.LoggedIn && loggedInResult.Message == "UserName is wrong!")
                        return BadRequest(loggedInResult);
                    else if(!loggedInResult.LoggedIn && loggedInResult.Message == "Password not matched")
                        return BadRequest(loggedInResult); 
                    else return Ok(loggedInResult);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e} thrown with message: {e.Message}");
                return StatusCode(500);
            }
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
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
                    var deleted = await _userService.DeleteUser(id);
                    if(deleted) return Ok();
                    return BadRequest();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e} thrown with message: {e.Message}");
                return StatusCode(500);
            }
        }

        [HttpGet("get")]
        public async Task<IActionResult> Get(CancellationToken ct)
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
                    var users = await _userService.ReturnUsers();
                    if(users.Any()) return Ok(users);
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