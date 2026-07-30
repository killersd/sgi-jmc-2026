namespace SGI_JMC.Api.Models;

// Reformulado a partir de SGI-JMC/Models/HorarioProfessor.cs.
// Em vez de 60 colunas soltas (s01, t02, tq03...), guardamos a grade semanal
// como JSON: { "manha": { "seg": ["","","","",""], "ter": [...], ... }, "tarde": {...} }
// Os dados do professor (nome, CPF, cargo, disciplinas) ficam no cadastro central
// de Professores — aqui só referenciamos pelo ProfessorId.
public class HorarioProfessor
{
    public int Id { get; set; }

    public int ProfessorId { get; set; }
    public Professor Professor { get; set; } = null!;

    // JSON da grade — ver GradeHorarioDto para o formato serializado/desserializado
    public string GradeJson { get; set; } = "{}";
}
