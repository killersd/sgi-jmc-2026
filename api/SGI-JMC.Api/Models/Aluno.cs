using System.ComponentModel.DataAnnotations;

namespace SGI_JMC.Api.Models;

// Migrado de SGI-JMC/Models/AlunoAtual.cs
public class Aluno
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Nome { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Pai { get; set; }

    [Required, MaxLength(200)]
    public string Mae { get; set; } = string.Empty;

    [Required]
    public DateTime DataNascimento { get; set; }

    [Required, MaxLength(300)]
    public string Endereco { get; set; } = string.Empty;

    [MaxLength(15)]
    public string? Telefone { get; set; }

    public int NumeroDeclaracao { get; set; }
    public string? CodigoAutenticacao { get; set; }

    [Required, MaxLength(50)]
    public string CodigoSeed { get; set; } = string.Empty;

    public DateTime? DataDeEmissao { get; set; }

    [Required]
    public int AnoLetivo { get; set; }

    public int? AnoSerie { get; set; }

    [MaxLength(1)]
    public string? Turma { get; set; }

    public int? FaseProSic { get; set; }
    public int? SerieOrigem { get; set; }

    [MaxLength(20)]
    public string? NumeroDoNis { get; set; }

    [Required, MaxLength(3)]
    public string CorrecaoDeFluxo { get; set; } = "Não";

    public bool Transferido { get; set; }

    public string? UrlFoto { get; set; }
}
