namespace SGI_JMC.Api.Models;

// Migrado de SGI-JMC/Models/Suspensao.cs — guarda cópia dos dados do aluno na emissão (histórico)
public class Suspensao
{
    public int Id { get; set; }

    public string NomeAluno { get; set; } = string.Empty;
    public string? NomePai { get; set; }
    public string NomeMae { get; set; } = string.Empty;
    public DateTime DataNascimento { get; set; }

    public string AnoSerie { get; set; } = string.Empty;
    public string Turma { get; set; } = string.Empty;
    public string Turno { get; set; } = string.Empty;
    public string CodigoSeed { get; set; } = string.Empty;

    public string DescricaoDoFato { get; set; } = string.Empty;
    public int Dias { get; set; }
    public int NumeroSuspensao { get; set; }

    public DateTime DataDeEmissao { get; set; }
    public string? EmitidoPor { get; set; }
}
