using Microsoft.AspNetCore.Mvc;
using MiniNova.BLL.Interfaces;

namespace MiniNova.API.Controllers
{
    [Route("api/operator")]
    [ApiController]
    public class OperatorController : ControllerBase
    {
        private readonly IOperatorService _operatorService;

        public OperatorController(IOperatorService operatorService)
        {
            _operatorService = operatorService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOperator(int id)
        {
            try
            {
                var oper = await _operatorService.GetOperatorByIdAsync(id);
                if (oper == null) return NotFound();

                return Ok(oper);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}