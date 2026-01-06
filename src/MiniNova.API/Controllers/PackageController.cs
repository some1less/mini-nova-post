using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
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
        public async Task<IActionResult> GetPackages([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        { 
            try
            {
                var result = await _packageService.GetAllAsync(page, pageSize);
                return Ok(result);
            } 
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPackage([FromRoute] int id)
        {
            try
            {
                var package = await _packageService.GetPackageByIdAsync(id);
                if (package == null) return NotFound();
                
                return Ok(package);
            } catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> PostPackage([FromBody] CreatePackageDTO package)
        {
            try
            {
                var created = await _packageService.CreatePackageAsync(package);
                return CreatedAtAction(nameof(GetPackage), new { id = created.Id }, created);
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
        public async Task<IActionResult> PutPackage(int id, [FromBody] UpdatePackageDTO updatePackage)
        {
            try
            {
                await _packageService.UpdatePackageAsync(updatePackage, id);
                return NoContent();
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
        public async Task<IActionResult> DeletePackage(int id)
        {
            try
            {
                await _packageService.DeletePackageAsync(id);
                return NoContent();
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


