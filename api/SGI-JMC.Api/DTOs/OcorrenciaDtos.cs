namespace SGI_JMC.Api.DTOs;

public record GerarAdvertenciaRequest(
    string CodigoSeed,
    string Turno,
    string DescricaoDoFato,
    int Numero
);

public record GerarSuspensaoRequest(
    string CodigoSeed,
    string Turno,
    string DescricaoDoFato,
    int Dias,
    int NumeroSuspensao
);

public record OcorrenciaEmitidaDto(
    int Id,
    string NomeAluno,
    string Tipo,
    DateTime DataDeEmissao
);
