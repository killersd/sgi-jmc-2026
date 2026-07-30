namespace SGI_JMC.Api.Models;

// Migrado de SGI-JMC/Models/OficioGeral.cs
public class Oficio
{
    public int Id { get; set; }

    public int NumeroOficio { get; set; }
    public string Assunto { get; set; } = string.Empty;
    public string Destinatario { get; set; } = string.Empty;
    public string CargoDoDestinatario { get; set; } = string.Empty;
    public string CorpoDoOficio { get; set; } = string.Empty;

    // "Vera" (Diretora) ou "Alex" (Secretário) — mesmas duas opções fixas do sistema original
    public string Remetente { get; set; } = string.Empty;

    public DateTime DataEmissao { get; set; }
    public string Cidade { get; set; } = string.Empty;
    public string? EmitidoPor { get; set; }
}
