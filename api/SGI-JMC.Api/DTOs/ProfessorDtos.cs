using System.ComponentModel.DataAnnotations;

namespace SGI_JMC.Api.DTOs;

public record DisciplinaDto(int Id, string Nome);
public record DisciplinaUpsertRequest([Required] string Nome);

public record ProfessorDto(
    int Id,
    string Nome,
    string CPF,
    string Cargo,
    int CargaHorariaSemanal,
    List<DisciplinaDto> Disciplinas
);

public record ProfessorUpsertRequest(
    [Required] string Nome,
    [Required] string CPF,
    [Required] string Cargo,
    int CargaHorariaSemanal,
    List<int> DisciplinaIds
);
