using MiniNova.BLL.DTO.Auth;

namespace MiniNova.BLL.Security.Auth;

public interface IAuthService
{
    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken);
    Task RegisterAsync(RegisterRequest request, CancellationToken cancellationToken);
}