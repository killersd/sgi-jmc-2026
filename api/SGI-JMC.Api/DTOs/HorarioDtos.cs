using System.ComponentModel.DataAnnotations;

namespace SGI_JMC.Api.DTOs;

public record TurmaDto(int Id, string Nome);
public record TurmaUpsertRequest([Required] string Nome);

// Cada célula da grade referencia a turma e a disciplina (em vez de texto livre)
public class CelulaHorarioDto
{
    public int? TurmaId { get; set; }
    public int? DisciplinaId { get; set; }
}

// Grade semanal: cada turno tem 6 dias, cada dia tem 5 aulas
public class GradeTurnoDto
{
    public CelulaHorarioDto[] Seg { get; set; } = CriarDiaVazio();
    public CelulaHorarioDto[] Ter { get; set; } = CriarDiaVazio();
    public CelulaHorarioDto[] Qua { get; set; } = CriarDiaVazio();
    public CelulaHorarioDto[] Qui { get; set; } = CriarDiaVazio();
    public CelulaHorarioDto[] Sex { get; set; } = CriarDiaVazio();
    public CelulaHorarioDto[] Sab { get; set; } = CriarDiaVazio();

    private static CelulaHorarioDto[] CriarDiaVazio() =>
        Enumerable.Range(0, 5).Select(_ => new CelulaHorarioDto()).ToArray();
}

public class GradeHorarioDto
{
    public GradeTurnoDto Manha { get; set; } = new();
    public GradeTurnoDto Tarde { get; set; } = new();
}

public record HorarioProfessorDto(
    int Id,
    int ProfessorId,
    string ProfessorNome,
    string CPF,
    string Cargo,
    int CargaHorariaSemanal,
    List<string> Disciplinas,
    GradeHorarioDto Grade
);

public record HorarioProfessorUpsertRequest(
    [Required] int ProfessorId,
    GradeHorarioDto Grade
);
