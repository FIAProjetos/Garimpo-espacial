using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Garimpo.Application.Ports;
using Garimpo.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Garimpo.Infrastructure.Security;

public sealed class JwtTokenService : ITokenService
{
    private readonly string _secret;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _expirationHours;

    public JwtTokenService(IConfiguration configuration)
    {
        _secret = configuration["Security:Jwt:Secret"]
            ?? throw new InvalidOperationException("Security:Jwt:Secret nao configurado.");
        _issuer = configuration["Security:Jwt:Issuer"] ?? "GarimpoEspacial";
        _audience = configuration["Security:Jwt:Audience"] ?? "GarimpoEspacialApp";
        _expirationHours = int.TryParse(configuration["Security:Jwt:ExpirationHours"], out int hours)
            ? hours
            : 24;

        if (_secret.Length < 32)
        {
            throw new InvalidOperationException("Security:Jwt:Secret deve ter pelo menos 32 caracteres.");
        }
    }

    public (string Token, DateTime ExpiresAt) GenerateToken(User user)
    {
        DateTime expiresAt = DateTime.UtcNow.AddHours(_expirationHours);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        string tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        return (tokenString, expiresAt);
    }
}
