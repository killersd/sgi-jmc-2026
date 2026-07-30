namespace SGI_JMC.Api.Models;

public class Professor
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string CPF { get; set; } = string.Empty;
    public string Cargo { get; set; } = string.Empty;
    public int CargaHorariaSemanal { get; set; }

    public ICollection<ProfessorDisciplina> ProfessorDisciplinas { get; set; } = new List<ProfessorDisciplina>();
}
