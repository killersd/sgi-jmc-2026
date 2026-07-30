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
