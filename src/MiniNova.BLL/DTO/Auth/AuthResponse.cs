namespace MiniNova.BLL.DTO.Auth;

public class AuthResponse
{
    public required string AccessToken { get; set; }
    public required string Login { get; set; }
}