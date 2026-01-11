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
            var result = await _authService.LoginAsync(request, cancellationToken);
            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request,
            CancellationToken cancellationToken)
        {
            await _authService.RegisterAsync(request, cancellationToken);
            return Ok(new { message = "Registration Successful" });
        }
    }
}

