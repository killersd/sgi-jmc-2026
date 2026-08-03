using System.ComponentModel.DataAnnotations;

namespace SGI_JMC.Api.DTOs;

public record ModuloDto(string Chave, string Nome);
public record PerfilDto(string Chave, string Nome);

public record PermissaoDto(string Perfil, string ModuloChave, bool Permitido);

public record AtualizarPermissoesRequest(List<PermissaoDto> Permissoes);

public record MatrizPermissoesDto(
    List<ModuloDto> Modulos,
    List<PerfilDto> PerfisEditaveis,
    List<PermissaoDto> Permissoes
);

public record UsuarioAdminDto(
    string Id,
    string NomeCompleto,
    string Email,
    List<string> Perfis,
    bool EmailConfirmado,
    bool Ativo,
    string? CPF,
    DateTime? DataNascimento
);

public record AtualizarPerfilUsuarioRequest(string NovoPerfil);

public record AtualizarUsuarioRequest(
    [Required] string NomeCompleto,
    [Required, EmailAddress] string Email,
    string? CPF,
    DateTime? DataNascimento
);

public record AtualizarStatusUsuarioRequest(bool Ativo);
