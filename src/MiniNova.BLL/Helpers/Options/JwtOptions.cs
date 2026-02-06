namespace MiniNova.BLL.Helpers.Options;

public class JwtOptions
{
    public required string Issuer { get; set; }
    public required string Audience { get; set; }
    public required int ValidInMinutes { get; set; }
    
}