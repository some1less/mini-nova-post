using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniNova.BLL.Interfaces;

namespace MiniNova.API.Controllers
{
    [Route("api/operators")]
    [ApiController]
    [Authorize]
    public class OperatorController : ControllerBase
    {
        private readonly IOperatorService _operatorService;

        public OperatorController(IOperatorService operatorService)
        {
            _operatorService = operatorService;
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
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
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}