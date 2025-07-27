using AccountManagementSystem.Services.IServices;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models;
using Models.DTOs;

namespace AccountManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IMapper _mapper;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IAccountService accountService, IMapper mapper, ILogger<AccountController> logger)
        {
            _accountService = accountService;
            _mapper = mapper;
            _logger = logger;
        }


        [HttpGet("AllAccounts")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var accounts = await _accountService.GetAccounts();
                if(accounts.Any()) return Ok(accounts);
                return NotFound();
            }
            catch (Exception e)  
            {
                _logger.LogError(e, "Exception thrown in Get AllAccounts: {Message}", e.Message);
                return StatusCode(500, "An internal server error occurred.");
            }
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
                cancellationToken.ThrowIfCancellationRequested();
                var account = await _accountService.GetSingleAccountWithRef(id);
                if(account != null) {
                    var accountWithAdharNumber = _mapper.Map<AccountReadDTO>(account);
                    return Ok(accountWithAdharNumber);
                }
                return NotFound();
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("GetAccountWithRef was canceled for id {AccountId}", id);
                return StatusCode(499, "Client closed request.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception thrown in GetAccountWithRef for id {AccountId}: {Message}", id, e.Message);
                return StatusCode(500, "An internal server error occurred.");
            }
        }

        [HttpGet("GetAccount/{id}")]
        public async Task<IActionResult> GetAccount(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                var account = await _accountService.GetSingleAccount(id);
                if(account != null) return Ok(account);
                return NotFound();
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("GetAccount was canceled for id {AccountId}", id);
                return StatusCode(499, "Client closed request.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception thrown in GetAccount for id {AccountId}: {Message}", id, e.Message);
                return StatusCode(500, "An internal server error occurred.");
            }
        }


        [HttpPost("Add")]
        public async Task<IActionResult> Create([FromBody]AccountModelInputDTO accountModelInputDTO, CancellationToken ct)
        {
            if (accountModelInputDTO == null)
            {
                return BadRequest("Account details cannot be null.");
            }
            
            try
            {
                ct.ThrowIfCancellationRequested();
                Account account = _mapper.Map<Account>(accountModelInputDTO);
                var newAccount = await _accountService.CreateNewAccount(account);
                if(newAccount != null) return StatusCode(201, newAccount);
                return BadRequest("Could not create the account with the provided details.");
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Create account operation was canceled.");
                return StatusCode(499, "Client closed request.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception thrown in Create: {Message}", e.Message);
                return StatusCode(500, "An internal server error occurred.");
            }
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody]UpdateAccountDTO updateAccount, CancellationToken ct)
        {
            if (updateAccount == null)
            {
                return BadRequest("Update details cannot be null.");
            }

            try
            {
                ct.ThrowIfCancellationRequested();
                var updatedAccount = await _accountService.UpdateAccount(id, updateAccount);
                if(updatedAccount != null) return Ok(updatedAccount);
                return BadRequest("Could not update the account with the provided details.");
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Update account was canceled for id {AccountId}", id);
                return StatusCode(499, "Client closed request.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception thrown in Update for id {AccountId}: {Message}", id, e.Message);
                return StatusCode(500, "An internal server error occurred.");
            }
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            try
            {
                ct.ThrowIfCancellationRequested();
                var updatedAccount = await _accountService.DeleteAccount(id);
                if(updatedAccount != null) return Ok(updatedAccount);
                return BadRequest("Could not delete the account.");
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Delete account was canceled for id {AccountId}", id);
                return StatusCode(499, "Client closed request.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception thrown in Delete for id {AccountId}: {Message}", id, e.Message);
                return StatusCode(500, "An internal server error occurred.");
            }
        }
    }
}
