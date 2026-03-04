using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace MiniNova.API.Extensions;

public static class AuthKeysExtension
{
    public static RsaSecurityKey AddAsyncKeyLoading(this IServiceCollection services, IConfiguration config)
    {
        // PUBLIC KEY PROCESSING
        var publicKeyBase64 = config["Jwt:PublicKeyBase64"];

        if (string.IsNullOrEmpty(publicKeyBase64))
            throw new Exception("[ERROR 500] Public key environment variable not found");

        var publicKeyPem = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(publicKeyBase64));
        
        var publicRsa = RSA.Create();
        publicRsa.ImportFromPem(publicKeyPem);
        var publicSecurityKey = new RsaSecurityKey(publicRsa);

        // PRIVATE KEY PROCESSING
        var privateKeyBase64 = config["Jwt:PrivateKeyBase64"];
        
        if (string.IsNullOrEmpty(privateKeyBase64))
            throw new Exception("[ERROR 500] Private key environment variable not found");

        var privateKeyPem = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(privateKeyBase64));

        var privateRsa = RSA.Create();
        privateRsa.ImportFromPem(privateKeyPem);
        var privateSecurityKey = new RsaSecurityKey(privateRsa);

        // MAIN PROCESSING
        var credentials = new SigningCredentials(privateSecurityKey, SecurityAlgorithms.RsaSha256);

        services.AddSingleton(publicSecurityKey);
        services.AddSingleton(credentials);

        return publicSecurityKey;
    }
}