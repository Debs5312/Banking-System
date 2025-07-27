using AdharManagementSystem.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace AdharManagementSystem.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdharController : ControllerBase
    {
        private readonly IAdharService _adharService;
        private readonly ILogger<AdharController> _logger;

        public AdharController(IAdharService adharService, ILogger<AdharController> logger)
        {
            _adharService = adharService;
            _logger = logger;
        }

        [HttpGet("AllAdhars")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var adhars = await _adharService.GetAdhars();
                if (adhars.Any())
                {
                    var adharNumberList = adhars.Select(item => item.Number).ToList();
                    return Ok(adharNumberList);
                }
                return NotFound();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception thrown in Get AllAdhars: {Message}", e.Message);
                return StatusCode(500, "An internal server error occurred.");
            }
        }

        [HttpGet("GetAdhar/{number}")]
        public async Task<IActionResult> GetAccount(int number, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                var adhar = await _adharService.GetSingleAdhar(number);
                if (adhar != null) return Ok(adhar.Id);
                return NotFound();
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("GetAccount was canceled for number {AdharNumber}", number);
                return StatusCode(499, "Client closed request.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception thrown in GetAccount for number {AdharNumber}: {Message}", number, e.Message);
                return StatusCode(500, "An internal server error occurred.");
            }
        }

        [HttpPost("Add/{adharNumber}")]
        public async Task<IActionResult> Create(int adharNumber, CancellationToken ct)
        {
            try
            {
                ct.ThrowIfCancellationRequested();
                var newAdhar = await _adharService.CreateNewAdhar(adharNumber);
                if (newAdhar != null) return StatusCode(201, newAdhar);
                return BadRequest();
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Create operation was canceled for adharNumber {AdharNumber}", adharNumber);
                return StatusCode(499, "Client closed request.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception thrown in Create for adharNumber {AdharNumber}: {Message}", adharNumber, e.Message);
                return StatusCode(500, "An internal server error occurred.");
            }
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] int updatedNumber, CancellationToken ct)
        {
            try
            {
                ct.ThrowIfCancellationRequested();
                var updatedAdhar = await _adharService.UpdateAdhar(id, updatedNumber);
                if (updatedAdhar != null) return Ok(updatedAdhar);
                return BadRequest();
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Update operation was canceled for id {AdharId}", id);
                return StatusCode(499, "Client closed request.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception thrown in Update for id {AdharId}: {Message}", id, e.Message);
                return StatusCode(500, "An internal server error occurred.");
            }
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            try
            {
                ct.ThrowIfCancellationRequested();
                var deletedAdhar = await _adharService.DeleteAdhar(id);
                if (deletedAdhar != null) return Ok(deletedAdhar);
                return BadRequest();
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Delete operation was canceled for id {AdharId}", id);
                return StatusCode(499, "Client closed request.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception thrown in Delete for id {AdharId}: {Message}", id, e.Message);
                return StatusCode(500, "An internal server error occurred.");
            }
        }
    }
}
