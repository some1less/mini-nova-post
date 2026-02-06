using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MiniNova.BLL.Helpers.Options;

namespace MiniNova.BLL.Security.Tokens;

public class TokenService : ITokenService
{
    private readonly JwtOptions _jwtConfig;
    private readonly SigningCredentials _signingCredentials;

    public TokenService(IOptions<JwtOptions> jwtConfig,  SigningCredentials signingCredentials)
    {
        _jwtConfig = jwtConfig.Value;
        _signingCredentials = signingCredentials;
    }
    
    public string GenerateToken(string login, string role, string email, int personId)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
       
        
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub, login),
            new(JwtRegisteredClaimNames.Name, login),
            
            new(ClaimTypes.Email, email), // auto pull-up while creating shipments
            new("userid", personId.ToString()), // auto pull-up while accessing and creating shipments
            
            new(ClaimTypes.Role, role) // verification of authorized access
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Issuer = _jwtConfig.Issuer,
            Audience = _jwtConfig.Audience,
            Expires = DateTime.UtcNow.Add(TimeSpan.FromMinutes(_jwtConfig.ValidInMinutes)),
            SigningCredentials =  _signingCredentials
        };
        
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwt = tokenHandler.WriteToken(token);
        return jwt;
    }
}