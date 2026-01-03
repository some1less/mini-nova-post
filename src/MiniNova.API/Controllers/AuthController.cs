using Microsoft.AspNetCore.Mvc;
using MiniNova.BLL.DTO.Auth;
using MiniNova.BLL.Security.Auth;

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
                return Unauthorized($"Access denied: {ex.Message}");
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
            } catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}

