using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ProductManager.Core.Domain.Entities;
using ProductManager.Core.DTOs.UserDTOs;
using ProductManager.Core.ServiceContracts;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace ProductManager.Core.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public AuthenticationResponse CreateToken(User user)
        {
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];
            var expiresAt =
                DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:EXPIRATION_TIME_IN_MINUTES"]));
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var claims = new Claim[]
            {
                new (JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new (JwtRegisteredClaimNames.Iat,
                    DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                    ClaimValueTypes.Integer64),
                new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new (ClaimTypes.Name, user.Email)
            };

            var signInCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenGenerator = new JwtSecurityToken(issuer, audience, claims, expires: expiresAt,
                signingCredentials: signInCredentials);

            var token = new JwtSecurityTokenHandler().WriteToken(tokenGenerator);

            return new AuthenticationResponse()
            {
                Email = user.Email,
                Name = user.Name,
                Expiration = expiresAt,
                Token = token
            };
        }
    }
}
