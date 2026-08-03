using Microsoft.AspNetCore.Identity;

namespace SGI_JMC.Api.Models;

// Reaproveita a estrutura do UsuarioModel original (Identity + campos extras)
public class ApplicationUser : IdentityUser
{
    public string NomeCompleto { get; set; } = string.Empty;
    public DateTime? DataNascimento { get; set; }
    public string? CPF { get; set; }
    public bool Ativo { get; set; } = true;
}
