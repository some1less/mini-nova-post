using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniNova.BLL.DTO.Tracking;
using MiniNova.BLL.Services.Tracking;

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
        public async Task<IActionResult> GetHistory(int id, CancellationToken cancellationToken)
        {
            var history = await _trackingService.GetHistoryByPackageIdAsync(id, cancellationToken);
            return Ok(history);
        }
        
        [HttpPost]
        [Authorize(Roles = "Admin, Operator")]
        public async Task<IActionResult> AddStatus([FromBody] TrackingDTO dto, CancellationToken cancellationToken)
        {
            var login = User.FindFirst("name")?.Value;
                
            if (string.IsNullOrEmpty(login)) 
                return Unauthorized(new { message = "User email not found in token" });

            var result = await _trackingService.AddTrackingAsync(dto, login, cancellationToken);
            return Ok(result);
        }
        
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Operator")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateTrackingDTO dto, CancellationToken cancellationToken)
        {
            await _trackingService.UpdateTrackingAsync(id, dto, cancellationToken);
            return Ok(new { message = "Tracking status updated successfully" });
        }
        
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteStatus(int id, CancellationToken cancellationToken)
        {
            await _trackingService.DeleteTrackingAsync(id, cancellationToken);
            return Ok(new { message = "Tracking record deleted successfully" });
        }
    }
}

