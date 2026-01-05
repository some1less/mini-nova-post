using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniNova.BLL.DTO.Tracking;
using MiniNova.BLL.Interfaces;

namespace MiniNova.API.Controllers
{
    [Route("api/trackings")]
    [ApiController]
    [Authorize]
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
        [Authorize(Roles = "Admin, Operator")]
        public async Task<IActionResult> AddStatus([FromBody] CreateTrackingDTO dto)
        {
            try
            {
                var login = User.Identity?.Name;
                
                if (string.IsNullOrEmpty(login)) 
                    return Unauthorized(new { message = "User email not found in token" });

                var result = await _trackingService.AddTrackingAsync(dto, login);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Operator")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateTrackingDTO dto)
        {
            try
            {
                await _trackingService.UpdateTrackingAsync(id, dto);
                return Ok(new { message = "Tracking status updated successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteStatus(int id)
        {
            try
            {
                await _trackingService.DeleteTrackingAsync(id);
                return Ok(new { message = "Tracking record deleted successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}

