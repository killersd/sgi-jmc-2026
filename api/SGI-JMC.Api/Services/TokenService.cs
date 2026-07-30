using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using SGI_JMC.Api.Models;

namespace SGI_JMC.Api.Services;

public interface ITokenService
{
    (string token, DateTime expiraEm) GerarToken(ApplicationUser usuario, IList<string> roles);
}

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public (string token, DateTime expiraEm) GerarToken(ApplicationUser usuario, IList<string> roles)
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var chave = jwtSettings["Key"]!;
        var horasExpiracao = double.Parse(jwtSettings["ExpiraEmHoras"] ?? "8");
        var expiraEm = DateTime.UtcNow.AddHours(horasExpiracao);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, usuario.Id),
            new(JwtRegisteredClaimNames.Email, usuario.Email ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("nomeCompleto", usuario.NomeCompleto)
        };
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var credenciais = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chave)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: expiraEm,
            signingCredentials: credenciais);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiraEm);
    }
}
