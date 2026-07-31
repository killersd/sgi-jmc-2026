namespace SGI_JMC.Api.DTOs;

public record GerarOficioGeralRequest(
    int NumeroOficio,
    string Assunto,
    string Destinatario,
    string CargoDoDestinatario,
    string CorpoDoOficio,
    string Remetente,
    string Cidade
);

public record OficioEmitidoDto(
    int Id,
    int NumeroOficio,
    string Assunto,
    string Destinatario,
    DateTime DataEmissao
);

public record GerarOficioFuncaoRequest(
    string Tipo, // "Professor" | "Servidor" | "ApoioEscolar"
    int NumeroOficio,
    string Assunto,
    string Destinatario,
    string CargoDestinatario,
    string CidadeDestinatario,
    string SaudacaoGenero, // "Senhor" | "Senhora"
    string Nome,
    string CPF,
    string Vinculo,
    DateTime DataAssumiuFuncao,
    int CargaHoraria,
    string? Cargo,
    string? Disciplina,
    int? FonteRecursos
);
