using System.ComponentModel.DataAnnotations;

namespace SGI_JMC.Api.DTOs;

public record LoginRequest(
    [Required, EmailAddress] string Email,
    [Required] string Senha
);

public record RegisterRequest(
    [Required] string NomeCompleto,
    [Required, EmailAddress] string Email,
    [Required, MinLength(6)] string Senha,
    string? CPF,
    DateTime? DataNascimento
);

public record AuthResponse(
    string Token,
    DateTime ExpiraEm,
    string Id,
    string Nome,
    string Email,
    IList<string> Roles
);
