using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniNova.BLL.DTO.Package;
using MiniNova.BLL.Interfaces;

namespace MiniNova.API.Controllers
{
    [Route("api/packages")]
    [ApiController]
    [Authorize]
    public class PackageController : ControllerBase
    {
        private readonly IPackageService _packageService;
        
        public PackageController(IPackageService packageService)
        {
            _packageService = packageService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Operator")]
        public async Task<IActionResult> GetPackages([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        { 
            var result = await _packageService.GetAllAsync(page, pageSize);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPackage([FromRoute] int id)
        {
            var package = await _packageService.GetPackageByIdAsync(id);
            return Ok(package);
        }
        
        [HttpPost]
        public async Task<IActionResult> PostPackage([FromBody] CreatePackageDTO package)
        {
            int? currentUserId = null;

            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                
            if (int.TryParse(userIdString, out int parsedId)) { 
                currentUserId = parsedId;
            }
            
            var created = await _packageService.CreatePackageAsync(package, currentUserId);    
            return CreatedAtAction(nameof(GetPackage), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Operator")]
        public async Task<IActionResult> PutPackage(int id, [FromBody] UpdatePackageDTO updatePackage)
        {
            await _packageService.UpdatePackageAsync(updatePackage, id);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePackage(int id)
        {
            await _packageService.DeletePackageAsync(id);
            return NoContent();
        }
        
        [HttpGet("my")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetMyPackages([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out int userId)) { 
                return Unauthorized(new { error = "Invalid token data" });
            }
            var result = await _packageService.GetUserPackagesAsync(userId, page, pageSize);
            return Ok(result);
        }

    }
}


