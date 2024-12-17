using AccountManagementSystem.Services.IServices;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.DTOs;

namespace AccountManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IMapper _mapper;

        public AccountController(IAccountService accountService, IMapper mapper)
        {
            _accountService = accountService;
            _mapper = mapper;
        }


        [HttpGet("AllAccounts")]
        public async Task<IActionResult> Get()
        {
            var accounts = await _accountService.GetAccounts();
            if(accounts.Any()) return Ok(accounts);
            return NotFound();
        }

        [HttpGet("GetAccount/{id}")]
        public async Task<IActionResult> GetAccount(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                if(cancellationToken.IsCancellationRequested)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    return NoContent();
                }
                else
                {
                    var account = await _accountService.GetSingleAccount(id);
                    if(account != null) return Ok(account);
                    return NotFound();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e} thrown with message: {e.Message}");
                return StatusCode(500);
            }
        }

        [HttpPost("AddNewAccount")]
        public async Task<IActionResult> CreateAccount([FromBody]AccountModelInputDTO accountModelInputDTO, CancellationToken ct)
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
                    Account account = _mapper.Map<Account>(accountModelInputDTO);
                    var newAccount = await _accountService.CreateNewAccount(account);
                    if(newAccount != null) return StatusCode(201, newAccount);
                    return BadRequest();
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