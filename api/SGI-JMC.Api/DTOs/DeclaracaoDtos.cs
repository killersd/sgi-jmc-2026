namespace SGI_JMC.Api.DTOs;

// Retornado ao pesquisar o aluno pelo código, antes de emitir a declaração
public record AlunoParaDeclaracaoDto(
    int Id,
    string Nome,
    string? Pai,
    string Mae,
    DateTime DataNascimento,
    string CodigoSeed,
    int AnoLetivo,
    int? AnoSerie,
    string? Turma,
    string? NumeroDoNis
);

public record GerarDeclaracaoRequest(string CodigoSeed, int QtdFaltas);

public record DeclaracaoEmitidaDto(
    int Id,
    string NomeAluno,
    int NumeroDeclaracao,
    string CodigoAutenticacao,
    DateTime DataDeEmissao
);
