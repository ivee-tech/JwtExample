using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Tokens
{
    public class JwtGen
    {
        public static string GenerateToken(
            string signingKey, 
            string nameId, 
            string audience, 
            string issuer, 
            string role = "Administrator", 
            IDictionary<string, string> additionalClaims = null,
            DateTime? issuedAt = null,
            DateTime? notBefore = null,
            int expireTime = 3600
        )
        {
            var secKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey));
            var signingCredentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(secKey,
                Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256Signature, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.Sha256Digest);

            var claimsIdentity = new ClaimsIdentity(new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, nameId),
                new Claim(ClaimTypes.Role, role),
            }, "Custom");

            if (additionalClaims != null)
            {
                claimsIdentity.AddClaims(additionalClaims.Select(kvp => new Claim(kvp.Key, kvp.Value)));
            }

            var tokenHandler = new JwtSecurityTokenHandler();

            var securityTokenDescriptor = new Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor()
            {
                Audience = audience,
                Issuer = issuer,
                Subject = claimsIdentity,
                SigningCredentials = signingCredentials,
                NotBefore = notBefore,
                IssuedAt = issuedAt,
                Expires = issuedAt.HasValue ? issuedAt.Value.AddSeconds(expireTime) : (DateTime?)null
            };

            var plainToken = tokenHandler.CreateToken(securityTokenDescriptor);
            var signedAndEncodedToken = tokenHandler.WriteToken(plainToken);


            return signedAndEncodedToken;
        }

        public static JwtSecurityToken ParseToken(string tokenData)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            if (tokenHandler.CanReadToken(tokenData))
            {
                var token = tokenHandler.ReadJwtToken(tokenData);
                return token;
            }
            return null;
        }

        public static ClaimsPrincipal ValidateToken(string tokenData, string signingKey, IEnumerable<string> audiences, IEnumerable<string> issuers)
        {
            var secKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey));
            var signingCredentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(secKey,
                Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256Signature, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.Sha256Digest);
            var tokenValidationParameters = new TokenValidationParameters()
            {
                ValidAudiences = audiences,
                ValidIssuers = issuers,
                IssuerSigningKey = secKey
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            Microsoft.IdentityModel.Tokens.SecurityToken validatedToken;
            var principal = tokenHandler.ValidateToken(tokenData,
                tokenValidationParameters, out validatedToken);

            return principal;
        }
    }
}
