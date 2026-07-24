using System.Security.Claims;
using System.Text;
using EcoFleetLogistics.Application.Authentication.Common;
using EcoFleetLogistics.Application.Common.Interfaces;
using EcoFleetLogistics.Domain.User;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace EcoFleetLogistics.Infrastructure.Persistence.Authentication;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtSettings _jwtSettings;
    
    public JwtTokenGenerator(IOptions<JwtSettings> jwtOptions)
    {
        _jwtSettings = jwtOptions.Value ?? throw new InvalidOperationException("JwtSettings:Secret key must be configured and at least 32 characters long.");
    }

    public string GenerateToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
        {
          new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
          new Claim(JwtRegisteredClaimNames.Email, user.Email.Value),
          new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName),
          new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName),
          new Claim(ClaimTypes.Role, user.Role.ToString()),
          new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())        
        }),
        Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
        Issuer= _jwtSettings.Issuer,
        Audience= _jwtSettings.Audience,
        SigningCredentials = credentials
        };

        return new JsonWebTokenHandler().CreateToken(tokenDescriptor);
    }
}