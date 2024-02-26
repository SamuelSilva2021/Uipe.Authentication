using Authentication.Entities;
using Authentication.Infrastructure.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Authentication.Core.Application.Auth.Implementation
{
    public class TokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        //public JwtSecurityToken CreateToken(AppUser user)
        //{
        //    var claims = new List<Claim>
        //        {
        //        new Claim(ClaimTypes.Name, user.UserName),
        //        new Claim(ClaimTypes.Email, user.Email),
        //        new Claim(ClaimTypes.NameIdentifier, user.Id)
        //        };

        //    using (RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider(2048))
        //    {
        //        var privateKey = new RsaSecurityKey(rSACryptoServiceProvider.ExportParameters(true));
        //        var credentials = new SigningCredentials(privateKey, SecurityAlgorithms.RsaSha256);

        //        var expiration = DateTime.UtcNow.AddHours(1);

        //        JwtSecurityToken token = new JwtSecurityToken(
        //            issuer: _configuration["Jwt:Issuer"],
        //            audience: _configuration["Jwt:Audience"],
        //            claims: claims,
        //            expires: expiration,
        //            signingCredentials: credentials
        //        );

        //        return token;
        //    }
        //}
        public JwtSecurityToken CreateToken(AppUser user)
        {
            UipeAuthOptions options = _configuration.GetSection("UipeAuth").Get<UipeAuthOptions>();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes($"{options.TokenConfigurations.Key}"));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: options.TokenConfigurations.Issuer,
                audience: options.TokenConfigurations.Audience,
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials
            );

            return token;
        }
    }
}
