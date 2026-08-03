using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SGI_JMC.Api.Authorization;
using SGI_JMC.Api.Data;
using SGI_JMC.Api.DTOs;
using SGI_JMC.Api.Models;

namespace SGI_JMC.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[RequerModulo(Modulos.Professores)]
public class ProfessoresController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProfessoresController(AppDbContext context)
    {
        _context = context;
    }

    private static ProfessorDto ParaDto(Professor p) => new(
        p.Id, p.Nome, p.CPF, p.Cargo, p.CargaHorariaSemanal,
        p.ProfessorDisciplinas
            .Select(pd => new DisciplinaDto(pd.Disciplina.Id, pd.Disciplina.Nome))
            .OrderBy(d => d.Nome)
            .ToList());

    // GET api/professores?busca=maria
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProfessorDto>>> Listar([FromQuery] string? busca)
    {
        var query = _context.Professores
            .Include(p => p.ProfessorDisciplinas).ThenInclude(pd => pd.Disciplina)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(busca))
            query = query.Where(p => p.Nome.Contains(busca));

        var itens = await query.OrderBy(p => p.Nome).ToListAsync();
        return Ok(itens.Select(ParaDto));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProfessorDto>> ObterPorId(int id)
    {
        var professor = await _context.Professores
            .Include(p => p.ProfessorDisciplinas).ThenInclude(pd => pd.Disciplina)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        return professor is null ? NotFound() : Ok(ParaDto(professor));
    }

    [HttpPost]
    public async Task<ActionResult<ProfessorDto>> Criar(ProfessorUpsertRequest request)
    {
        var professor = new Professor
        {
            Nome = request.Nome,
            CPF = request.CPF,
            Cargo = request.Cargo,
            CargaHorariaSemanal = request.CargaHorariaSemanal,
        };

        professor.ProfessorDisciplinas = request.DisciplinaIds
            .Distinct()
            .Select(disciplinaId => new ProfessorDisciplina { DisciplinaId = disciplinaId })
            .ToList();

        _context.Professores.Add(professor);
        await _context.SaveChangesAsync();

        return await ObterPorId(professor.Id);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ProfessorDto>> Atualizar(int id, ProfessorUpsertRequest request)
    {
        var professor = await _context.Professores
            .Include(p => p.ProfessorDisciplinas)
            .FirstOrDefaultAsync(p => p.Id == id);
        if (professor is null) return NotFound();

        professor.Nome = request.Nome;
        professor.CPF = request.CPF;
        professor.Cargo = request.Cargo;
        professor.CargaHorariaSemanal = request.CargaHorariaSemanal;

        // Substitui os vínculos de disciplina pelos enviados
        _context.RemoveRange(professor.ProfessorDisciplinas);
        professor.ProfessorDisciplinas = request.DisciplinaIds
            .Distinct()
            .Select(disciplinaId => new ProfessorDisciplina { ProfessorId = id, DisciplinaId = disciplinaId })
            .ToList();

        await _context.SaveChangesAsync();
        return await ObterPorId(id);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "administrador")]
    public async Task<IActionResult> Remover(int id)
    {
        var professor = await _context.Professores.FirstOrDefaultAsync(p => p.Id == id);
        if (professor is null) return NotFound();

        var emUso = await _context.HorariosProfessor.AnyAsync(h => h.ProfessorId == id);
        if (emUso)
            return Conflict(new { mensagem = "Este professor já possui um horário cadastrado. Remova o horário primeiro." });

        _context.Professores.Remove(professor);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
