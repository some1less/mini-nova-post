namespace MiniNova.BLL.Security.Tokens;

public interface ITokenService
{
    string GenerateToken(string login, string role);
}