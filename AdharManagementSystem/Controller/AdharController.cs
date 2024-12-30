using AdharManagementSystem.Services.IServices;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AdharManagementSystem.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdharController : ControllerBase
    {
        private readonly IAdharService _adharService;
        private readonly IMapper _mapper;

        public AdharController(IAdharService adharService)
        {
            _adharService = adharService;
        }

        [HttpGet("AllAdhars")]
        public async Task<IActionResult> Get()
        {
            var adhars = await _adharService.GetAdhars();
            if(adhars.Any())
            {
                var adharNumberList = adhars.Select(item => item.Number).ToList();
                return Ok(adharNumberList);
            }
            return NotFound();
        }

        [HttpGet("GetAdhar/{number}")]
        public async Task<IActionResult> GetAccount(int number, CancellationToken cancellationToken)
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
                    var adhar = await _adharService.GetSingleAdhar(number);
                    if(adhar != null) return Ok(adhar.Number);
                    return NotFound();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e} thrown with message: {e.Message}");
                return StatusCode(500);
            }
        }

        [HttpPost("Add/{adharNumber}")]
        public async Task<IActionResult> Create(int adharNumber, CancellationToken ct)
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
                    var newAdhar = await _adharService.CreateNewAdhar(adharNumber);
                    if(newAdhar != null) return StatusCode(201, newAdhar);
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
        public async Task<IActionResult> Update(Guid id, [FromBody]int updatedNumber, CancellationToken ct)
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
                    var updatedAdhar = await _adharService.UpdateAdhar(id, updatedNumber);
                    if(updatedAdhar != null) return Ok(updatedAdhar);
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
                    var updatedAdhar = await _adharService.DeleteAdhar(id);
                    if(updatedAdhar != null) return Ok(updatedAdhar);
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