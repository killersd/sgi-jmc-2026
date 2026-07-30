using System.ComponentModel.DataAnnotations;

namespace SGI_JMC.Api.DTOs;

public record AlunoDto(
    int Id,
    string Nome,
    string? Pai,
    string Mae,
    DateTime DataNascimento,
    string Endereco,
    string? Telefone,
    string CodigoSeed,
    int AnoLetivo,
    int? AnoSerie,
    string? Turma,
    string? NumeroDoNis,
    string CorrecaoDeFluxo,
    bool Transferido,
    string? UrlFoto
);

public record AlunoUpsertRequest(
    [Required, MaxLength(200)] string Nome,
    string? Pai,
    [Required, MaxLength(200)] string Mae,
    [Required] DateTime DataNascimento,
    [Required, MaxLength(300)] string Endereco,
    string? Telefone,
    [Required, MaxLength(50)] string CodigoSeed,
    [Required] int AnoLetivo,
    int? AnoSerie,
    string? Turma,
    string? NumeroDoNis,
    [Required] string CorrecaoDeFluxo,
    bool Transferido
);

// Resultado paginado, usado pela listagem de alunos
public record PagedResult<T>(IEnumerable<T> Items, int TotalCount, int Page, int PageSize);
