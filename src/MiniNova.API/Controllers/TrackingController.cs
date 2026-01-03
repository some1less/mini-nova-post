using Microsoft.AspNetCore.Mvc;
using MiniNova.BLL.DTO.Tracking;
using MiniNova.BLL.Interfaces;

namespace MiniNova.API.Controllers
{
    [Route("api/trackings")]
    [ApiController]
    public class TrackingController : ControllerBase
    {
        
        private readonly ITrackingService _trackingService;
    
        public  TrackingController(ITrackingService trackingService)
        {
            _trackingService = trackingService;
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetHistory(int id)
        {
            try
            {
                var history = await _trackingService.GetHistoryByPackageIdAsync(id);
                return Ok(history);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> AddStatus([FromBody] CreateTrackingDTO dto)
        {
            try
            {
                var email = User.Identity?.Name;
                if (email == null) return Unauthorized();

                var result = await _trackingService.AddTrackingAsync(dto, email);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

