using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using MiniNova.BLL.DTO.Auth;
using MiniNova.BLL.Security.Auth;
using ValidationException = MiniNova.BLL.Exceptions.ValidationException;

namespace MiniNova.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _authService.LoginAsync(request, cancellationToken);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex) 
            {
                return Unauthorized(new { error = ex.Message });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                await _authService.RegisterAsync(request, cancellationToken);
                return Ok(new { message = "Registration Successful" });
            }
            catch (ValidationException ex)
            {
                ModelState.AddModelError(ex.FieldName, ex.Message);
                return ValidationProblem(ModelState);
            } 
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}

