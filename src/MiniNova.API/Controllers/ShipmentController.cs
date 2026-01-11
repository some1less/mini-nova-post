using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniNova.BLL.DTO.Package;
using MiniNova.BLL.Interfaces;

namespace MiniNova.API.Controllers
{
    [Route("api/shipments")]
    [ApiController]
    public class ShipmentController : ControllerBase
    {
        private readonly IShipmentService _shipmentService;
        
        public ShipmentController(IShipmentService shipmentService)
        {
            _shipmentService = shipmentService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Operator")] 
        public async Task<IActionResult> GetShipments(CancellationToken cancellationToken, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        { 
            var result = await _shipmentService.GetAllAsync(cancellationToken, page, pageSize);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetShipment([FromRoute] int id, CancellationToken cancellationToken)
        {
            var package = await _shipmentService.GetShipmentByIdAsync(id, cancellationToken);
            return Ok(package);
        }
        
        [HttpGet("tracking/{trackingNumber}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByTrackingNumber([FromRoute] string trackingNumber, CancellationToken cancellationToken)
        {
            var result = await _shipmentService.GetShipmentByTrackingNumberAsync(trackingNumber, cancellationToken);
            return Ok(result);
        }
        
        [HttpPost]
        public async Task<IActionResult> PostShipment([FromBody] CreateShipmentDTO package, CancellationToken cancellationToken)
        {
            int? currentUserId = null;

            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                
            if (int.TryParse(userIdString, out int parsedId)) { 
                currentUserId = parsedId;
            }
            
            var created = await _shipmentService.CreateShipmentAsync(package, cancellationToken, currentUserId);    
            return CreatedAtAction(nameof(GetShipment), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Operator")]
        public async Task<IActionResult> PutShipment(int id, [FromBody] UpdateShipmentDTO updatePackage, CancellationToken cancellationToken)
        {
            await _shipmentService.UpdateShipmentAsync(updatePackage, id, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteShipment(int id, CancellationToken cancellationToken)
        {
            await _shipmentService.DeleteShipmentAsync(id, cancellationToken);
            return NoContent();
        }
        
        [HttpGet("my")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetMyShipments(CancellationToken cancellationToken, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out int userId)) { 
                return Unauthorized(new { error = "Invalid token data" });
            }
            var result = await _shipmentService.GetUserShipmentsAsync(userId, cancellationToken, page, pageSize);
            return Ok(result);
        }
    }
    
}


