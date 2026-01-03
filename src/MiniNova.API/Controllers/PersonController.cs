using Microsoft.AspNetCore.Mvc;
using MiniNova.BLL.DTO.People;
using MiniNova.BLL.Interfaces;
using MiniNova.DAL.Context;
using MiniNova.DAL.Models;

namespace MiniNova.API.Controllers
{
    [Route("api/people")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        private readonly IPersonService _personService;
        
        public PersonController(IPersonService personService)
        {
            _personService = personService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPeople()
        {
            try
            {
                var people =  await _personService.GetAllAsync();
                return Ok(people);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPerson(int id)
        {
            try
            {
                var person = await _personService.GetPersonByIdAsync(id);
                if (person == null) return NotFound();
                
                return Ok(person);
                
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> PostPerson([FromBody] CreatePersonDTO person)
        {
            try
            {
                var created = await _personService.CreatePersonAsync(person);
                return CreatedAtAction(nameof(GetPerson), new { id = created.Id }, created);
            }  catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPerson(int id, [FromBody] UpdatePersonDTO updatePerson)
        {
            try
            {
                await _personService.UpdatePersonAsync(updatePerson, id);
                return NoContent();

            } catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePerson(int id)
        {
            try
            {
                await _personService.DeletePersonAsync(id);
                return NoContent();
            }  catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
    }
}

