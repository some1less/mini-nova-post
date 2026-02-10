using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace MiniNova.API.Extensions;

public static class AuthKeysExtension
{
    public static RsaSecurityKey AddAsyncKeyLoading(this IServiceCollection services)
    {
        // PUBLIC KEY PROCESSING
        var publicKeyPath = Path.Combine(AppContext.BaseDirectory, "Keys", "public_key.pem");
        if (!File.Exists(publicKeyPath))
        {
            throw new FileNotFoundException("[ERROR 500] Public key not found");
        }

        var publicKey = File.ReadAllText(publicKeyPath);

        var publicRsa = RSA.Create();
        publicRsa.ImportFromPem(publicKey);
        var publicSecurityKey = new RsaSecurityKey(publicRsa);

        // PRIVATE KEY PROCESSING
        var privateKeyPath = Path.Combine(AppContext.BaseDirectory, "Keys", "private_key.pem");
        if (!File.Exists(privateKeyPath))
        {
            throw new FileNotFoundException("[ERROR 500] Private key not found");
        }

        var privateKey = File.ReadAllText(privateKeyPath);

        var privateRsa = RSA.Create();
        privateRsa.ImportFromPem(privateKey);
        var privateSecurityKey = new RsaSecurityKey(privateRsa);

        // MAIN PROCESSING
        var credentials = new SigningCredentials(privateSecurityKey, SecurityAlgorithms.RsaSha256);

        services.AddSingleton(publicSecurityKey);
        services.AddSingleton(credentials);

        return publicSecurityKey;
    }
}