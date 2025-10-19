using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Auth.Application.Interfaces.Tokens;
using Auth.Common.Configuration;
using Auth.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Auth.Infrastructure.Tokens;

public class TokensService(IOptions<JwtSettings> jwtSettings) : ITokensService
{
    public string Generate(UserEntity user)
    {
        var claims = new List<Claim>
        {
            new (ClaimTypes.NameIdentifier, user.Id),
            new (ClaimTypes.Name, user.Username),
            new (ClaimTypes.Role, user.Role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Value.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
      
        var token = new JwtSecurityToken(
            issuer: jwtSettings.Value.Issuer,
            audience: jwtSettings.Value.Audience,
            claims: claims,
            expires: DateTime.Now.AddHours(jwtSettings.Value.ExpirationHours),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}