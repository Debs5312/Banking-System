using Microsoft.AspNetCore.Mvc;
using Models.DTOs;
using Microsoft.Extensions.Logging;
using UserManagementSystem.Service.IService;

namespace UserManagementSystem.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        // Registration endpoint
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]RegistrationDTO registrationDTO, CancellationToken ct)
        {
            try
            {
                ct.ThrowIfCancellationRequested();

                var registrationStatus = await _userService.RegisterNewUser(registrationDTO);

                return registrationStatus switch
                {
                    RegistrationResult.Success => Ok("User registered successfully."),
                    RegistrationResult.UserNameExists => Conflict("A user with this username already exists."),
                    RegistrationResult.AdharIdInUse => Conflict("This Adhar ID is already in use."),
                    RegistrationResult.InvalidAdharId => BadRequest("The provided Adhar ID is not a valid format."),
                    _ => BadRequest("An unexpected error occurred during registration.")
                };
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Registration operation was canceled.");
                return new EmptyResult(); // Or StatusCode(499) if your client handles it
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred during user registration.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An internal error occurred.");
            }
        }

        // Login endpoint
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]LoginDTO loginDTO, CancellationToken ct)
        {
            try
            {
                ct.ThrowIfCancellationRequested();

                var loggedInResult = await _userService.LoginUser(loginDTO);

                if (loggedInResult.LoggedIn)
                {
                    return Ok(loggedInResult);
                }

                return Unauthorized(loggedInResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred during user login.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An internal error occurred.");
            }
        }

        // Delete endpoint
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            try
            {
                ct.ThrowIfCancellationRequested();

                var deleted = await _userService.DeleteUser(id);

                return deleted ? Ok("User deleted successfully.") : NotFound("User not found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred during user deletion for ID {UserId}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "An internal error occurred.");
            }
        }

        [HttpGet("get")]
        public async Task<IActionResult> Get(CancellationToken ct)
        {
            try
            {
                ct.ThrowIfCancellationRequested();

                var users = await _userService.ReturnUsers();
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred while retrieving users.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An internal error occurred.");
            }
        }
    }
}