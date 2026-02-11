using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniNova.BLL.DTO.Shipment;
using MiniNova.BLL.Services.Shipment;

namespace MiniNova.API.Controllers
{
    [Route("api/shipments")]
    [ApiController]
    [Authorize]
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
            var shipment = await _shipmentService.GetShipmentByIdAsync(id, cancellationToken);
            if (shipment == null) return NotFound();
            return Ok(shipment);
        }
        
        [HttpGet("tracking/{trackingNumber}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByTrackingNumber([FromRoute] string trackingNumber, CancellationToken cancellationToken)
        {
            var result = await _shipmentService.GetShipmentByTrackingNumberAsync(trackingNumber, cancellationToken);
            return Ok(result);
        }
        
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PostShipment([FromBody] CreateShipmentDTO shipmentDto, CancellationToken cancellationToken)
        {
            var userIdString = User.FindFirst("userid")?.Value;
            var userId = int.Parse(userIdString!);
            
            var created = await _shipmentService.CreateShipmentAsync(shipmentDto, cancellationToken, userId);    
            return CreatedAtAction(nameof(GetShipment), new { id = created!.Id }, created);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Operator")]
        public async Task<IActionResult> PutShipment(int id, [FromBody] UpdateShipmentDTO shipmentDto, CancellationToken cancellationToken)
        {
            await _shipmentService.UpdateShipmentAsync(shipmentDto, id, cancellationToken);
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
            var userId = User.FindFirst("userid")?.Value;
            var userIdInt = int.Parse(userId!);
            
            var result = await _shipmentService.GetUserShipmentsAsync(userIdInt, cancellationToken, page, pageSize);
            return Ok(result);
        }
    }
    
}


