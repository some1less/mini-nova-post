using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MiniNova.BLL.DTO.Package;
using MiniNova.BLL.Interfaces;
using MiniNova.BLL.Services;

namespace MiniNova.API.Controllers
{
    [Route("api/packages")]
    [ApiController]
    [Authorize]
    public class PackageController : ControllerBase
    {
        private readonly IPackageService _packageService;
        private readonly IPersonService _personService;
        
        public PackageController(IPackageService packageService, IPersonService personService)
        {
            _packageService = packageService;
            _personService = personService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Operator")]
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
        [Authorize(Roles = "Operator")]
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
        [Authorize(Roles = "Admin")]
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
        
        [HttpGet("my")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetMyPackages([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var login = User.FindFirst("name")?.Value 
                            ?? User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value 
                            ?? User.Identity?.Name;

                if (string.IsNullOrEmpty(login)) return Unauthorized();

                var personId = await _personService.GetPersonIdByLoginAsync(login);
                if (personId == null) return Unauthorized(new { error = "Profile not found" });

                var result = await _packageService.GetUserPackagesAsync(personId.Value, page, pageSize);
        
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

    }
}


