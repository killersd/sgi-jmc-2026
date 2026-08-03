using System.ComponentModel.DataAnnotations;

namespace SGI_JMC.Api.DTOs;

public record LoginRequest(
    [Required, EmailAddress] string Email,
    [Required] string Senha
);

public record RegisterRequest(
    [Required] string NomeCompleto,
    [Required, EmailAddress] string Email,
    [Required] string Perfil,
    string? CPF,
    DateTime? DataNascimento
);

public record AuthResponse(
    string Token,
    DateTime ExpiraEm,
    string Id,
    string Nome,
    string Email,
    IList<string> Roles,
    List<string> ModulosPermitidos
);

public record UsuarioCriadoDto(
    string Id,
    string NomeCompleto,
    string Email,
    bool EmailEnviado
);

public record AtivarContaRequest(
    [Required] string UserId,
    [Required] string Token,
    [Required, MinLength(6)] string NovaSenha
);
