using System;
using System.Collections.Generic;
using System.Linq;
using System.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.CommonLib;
using System.Configuration;
using Microsoft.ConfigReaders;
using Microsoft.Tokens;

namespace JwtExample
{
    class Program
    {
        static void Main(string[] args)
        {
            var nameId = "myemail@provider.com";
            var issuer = "http://my.tokenissuer.com";
            var audience = "http://my.website.com";
            var role = "User";
            var subscription = Guid.NewGuid().ToString();

            var appConfig = new AppSettingsConfigReader();
            var kvUri = appConfig["KeyVaultUri"];

            IConfigReader kvConfig = new CredentialsKeyVaultConfigReader(appConfig);
            var signingKey = kvConfig["JwtExample-SigningKey"];
            Console.WriteLine("Signing key: {0}", signingKey);

            var tokenData = JwtGen.GenerateToken(signingKey, nameId, audience, issuer, role, new Dictionary<string, string> { { nameof(subscription), subscription } });
            Console.WriteLine("Token data: {0}", tokenData);

            var token = JwtGen.ParseToken(tokenData);
            Console.WriteLine("Audience: {0}", token.Audiences.First());
            Console.WriteLine("Subscription: {0}", token.Claims.SingleOrDefault(c => c.Type == nameof(subscription)).Value);

            var principal = JwtGen.ValidateToken(tokenData, signingKey, new string[] { audience }, new string[] { issuer });
            Console.WriteLine("Principal: {0}", principal.Identity.Name);
            Console.WriteLine("Subscription: {0}", principal.Claims.SingleOrDefault(c => c.Type == nameof(subscription)).Value);

            Console.ReadLine();
        }
    }
}
