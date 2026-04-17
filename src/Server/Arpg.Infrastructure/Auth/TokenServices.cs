using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Arpg.Application.Auth;
using Arpg.Core.Models.Customer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Arpg.Infrastructure.Auth;

public class TokenServices(IConfiguration configuration) : ITokenServices
{
    public string GenerateToken(User user, string username)
    {
        var key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!);

        var tokenConfig = new SecurityTokenDescriptor
        {
            Subject = new(
            [
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, username)
            ]),
            Expires = DateTime.UtcNow.AddHours(8),
            SigningCredentials = new
            (
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            )
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenConfig);

        return tokenHandler.WriteToken(token);
    }
}