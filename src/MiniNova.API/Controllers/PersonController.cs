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
        public async Task<IActionResult> GetPeople(CancellationToken cancellationToken, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _personService.GetAllAsync(cancellationToken, page, pageSize);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, Operator, User")]
        public async Task<IActionResult> GetPerson(int id, CancellationToken cancellationToken)
        {
            
            var currentLogin = User.FindFirst("name")?.Value;
            if (currentLogin == null) throw new Exception("Cannot retrieve user name error");
                
            var hasFullAccess = User.IsInRole("Admin") || User.IsInRole("Operator");

            if (!hasFullAccess)
            {
                var requestingPersonId = await _personService.GetPersonIdByLoginAsync(currentLogin,  cancellationToken);
                    
                if (requestingPersonId == null) 
                    return Unauthorized(new { error = "User profile not found" });

                if (requestingPersonId != id) 
                    return Forbid();
            }

            var person = await _personService.GetPersonByIdAsync(id, cancellationToken);
            if (person == null) return NotFound(new { error = $"Person with id {id} not found" });
                
            return Ok(person);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Operator, User")]
        public async Task<IActionResult> PutPerson(int id, [FromBody] PersonDTO updatePerson, CancellationToken cancellationToken)
        {
            var currentLogin = User.FindFirst("name")?.Value;
                
            var fullAccess = User.IsInRole("Admin") || User.IsInRole("Operator");

            if (!fullAccess)
            {
                var currentPersonId = await _personService.GetPersonIdByLoginAsync(currentLogin!, cancellationToken);
                if (currentPersonId == null) return Unauthorized(new { error = "User profile not found" });

                if (currentPersonId != id) return Forbid();
            }
            
            await _personService.UpdatePersonAsync(updatePerson, id, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePerson(int id, CancellationToken cancellationToken)
        {
            await _personService.DeletePersonAsync(id, cancellationToken);
            return NoContent();
        }
    }
}

