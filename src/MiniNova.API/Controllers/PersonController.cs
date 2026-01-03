using Microsoft.AspNetCore.Mvc;
using MiniNova.BLL.Interfaces;
using MiniNova.DAL.Context;

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
                
                if (person == null)
                {
                    return NotFound();
                }
                return Ok(person);
                
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
    }
}

