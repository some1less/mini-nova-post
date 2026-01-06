using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniNova.BLL.DTO.People;
using MiniNova.BLL.Interfaces;
using MiniNova.DAL.Context;
using MiniNova.DAL.Models;

namespace MiniNova.API.Controllers
{
    [Route("api/people")]
    [ApiController]
    [Authorize]
    public class PersonController : ControllerBase
    {
        private readonly IPersonService _personService;
        
        public PersonController(IPersonService personService)
        {
            _personService = personService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPeople([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _personService.GetAllAsync(page, pageSize);
                return Ok(result);
            } 
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, Operator, User")]
        public async Task<IActionResult> GetPerson(int id)
        {
            try
            {
                var currentLogin = User.FindFirst("name")?.Value;
                
                var hasFullAccess = User.IsInRole("Admin") || User.IsInRole("Operator");

                if (!hasFullAccess)
                {
                    var requestingPersonId = await _personService.GetPersonIdByLoginAsync(currentLogin!);
                    
                    if (requestingPersonId == null) 
                        return Unauthorized(new { error = "User profile not found" });

                    if (requestingPersonId != id) 
                        return Forbid();
                }

                var person = await _personService.GetPersonByIdAsync(id);
                if (person == null) return NotFound(new { error = $"Person with id {id} not found" });
                
                return Ok(person);
                
            } catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Operator, User")]
        public async Task<IActionResult> PutPerson(int id, [FromBody] PersonDTO updatePerson)
        {
            try
            {
                var currentLogin = User.FindFirst("name")?.Value;
                
                var fullAccess = User.IsInRole("Admin") || User.IsInRole("Operator");

                if (!fullAccess)
                {
                    var currentPersonId = await _personService.GetPersonIdByLoginAsync(currentLogin!);
                    if (currentPersonId == null) return Unauthorized(new { error = "User profile not found" });

                    if (currentPersonId != id) return Forbid();
                }
                
                
                await _personService.UpdatePersonAsync(updatePerson, id);
                return NoContent();

            } catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            } catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePerson(int id)
        {
            try
            {
                await _personService.DeletePersonAsync(id);
                return NoContent();
            }  catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            } catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        
    }
}

