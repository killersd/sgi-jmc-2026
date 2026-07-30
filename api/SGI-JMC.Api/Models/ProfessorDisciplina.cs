namespace SGI_JMC.Api.Models;

public class ProfessorDisciplina
{
    public int ProfessorId { get; set; }
    public Professor Professor { get; set; } = null!;

    public int DisciplinaId { get; set; }
    public Disciplina Disciplina { get; set; } = null!;
}
