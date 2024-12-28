using AccountManagementSystem.Services.IServices;
using AutoMapper;
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

        [HttpGet("AllAccounts/withRef")]
        public async Task<IActionResult> GetWithRef()
        {
            var accounts = await _accountService.GetAccountsWithRef();
            if(accounts.Any())
            {
                var readAccountDetails = _mapper.Map<List<AccountReadDTO>>(accounts);
                return Ok(readAccountDetails);
            } 
            return NotFound();
        }

        [HttpGet("GetAccount/withRef/{id}")]
        public async Task<IActionResult> GetAccountWithRef(Guid id, CancellationToken cancellationToken)
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
                    var account = await _accountService.GetSingleAccountWithRef(id);
                    if(account != null) {
                        var accountWithAdharNumber = _mapper.Map<AccountReadDTO>(account);
                        return Ok(accountWithAdharNumber);
                    }
                    return NotFound();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e} thrown with message: {e.Message}");
                return StatusCode(500);
            }
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


        [HttpPost("Add")]
        public async Task<IActionResult> Create([FromBody]AccountModelInputDTO accountModelInputDTO, CancellationToken ct)
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

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody]UpdateAccountDTO updateAccount, CancellationToken ct)
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
                    var updatedAccount = await _accountService.UpdateAccount(id, updateAccount);
                    if(updatedAccount != null) return Ok(updatedAccount);
                    return BadRequest();
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
                    var updatedAccount = await _accountService.DeleteAccount(id);
                    if(updatedAccount != null) return Ok(updatedAccount);
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