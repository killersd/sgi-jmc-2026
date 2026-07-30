namespace SGI_JMC.Api.Models;

// Migrado de SGI-JMC/Models/Declaracao.cs — guarda uma cópia dos dados do aluno
// no momento da emissão (histórico), igual ao comportamento do sistema original.
public class Declaracao
{
    public int Id { get; set; }

    public string NomeAluno { get; set; } = string.Empty;
    public string? NomePai { get; set; }
    public string NomeMae { get; set; } = string.Empty;
    public DateTime DataNascimento { get; set; }

    public int AnoLetivo { get; set; }
    public int AnoSerie { get; set; }
    public string Turma { get; set; } = string.Empty;
    public string? NumeroDoNis { get; set; }
    public string CodigoSeed { get; set; } = string.Empty;

    public int QtdFaltas { get; set; }

    public int NumeroDeclaracao { get; set; }
    public string CodigoAutenticacao { get; set; } = string.Empty;

    public DateTime DataDeEmissao { get; set; }
    public string? EmitidoPor { get; set; }
}
