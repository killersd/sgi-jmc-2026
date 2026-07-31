namespace SGI_JMC.Api.Models;

// Consolida SGI-JMC/Models/OficioAssumiuFuncao.cs, OficioAssumiuFuncaoServidor.cs e
// OficioAssumiuFuncaoApoioEscolar2.cs num único modelo, diferenciado pelo campo Tipo.
public class OficioFuncao
{
    public int Id { get; set; }

    // "Professor" | "Servidor" | "ApoioEscolar"
    public string Tipo { get; set; } = string.Empty;

    public int NumeroOficio { get; set; }
    public string Assunto { get; set; } = string.Empty;
    public string Destinatario { get; set; } = string.Empty;
    public string CargoDestinatario { get; set; } = string.Empty;
    public string CidadeDestinatario { get; set; } = string.Empty;

    // "Senhor" | "Senhora" — substitui a checagem por nome fixo do sistema original
    public string SaudacaoGenero { get; set; } = "Senhor";

    public string Nome { get; set; } = string.Empty;
    public string CPF { get; set; } = string.Empty;
    public string Vinculo { get; set; } = string.Empty;
    public DateTime DataAssumiuFuncao { get; set; }
    public int CargaHoraria { get; set; }

    // Só usado quando Tipo == "Servidor" (cargo livre)
    public string? Cargo { get; set; }

    // Só usado quando Tipo == "Professor"
    public string? Disciplina { get; set; }
    public int? FonteRecursos { get; set; }

    public DateTime DataEmissao { get; set; }
    public string? EmitidoPor { get; set; }
}
