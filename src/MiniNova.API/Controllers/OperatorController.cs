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

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetOperators(CancellationToken cancellationToken)
        {
            var operators = await _operatorService.GetAllOperatorsAsync(cancellationToken);
            return Ok(operators);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetOperator(int id, CancellationToken cancellationToken)
        {
            var oper = await _operatorService.GetOperatorByIdAsync(id, cancellationToken);
            if (oper == null) return NotFound();

            return Ok(oper);
        }
    }
}