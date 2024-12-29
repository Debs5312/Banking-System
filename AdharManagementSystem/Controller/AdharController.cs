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


    }
}