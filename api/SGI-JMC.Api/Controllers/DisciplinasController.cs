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
public class DisciplinasController : ControllerBase
{
    private readonly AppDbContext _context;

    public DisciplinasController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DisciplinaDto>>> Listar()
    {
        var itens = await _context.Disciplinas.AsNoTracking()
            .OrderBy(d => d.Nome)
            .Select(d => new DisciplinaDto(d.Id, d.Nome))
            .ToListAsync();

        return Ok(itens);
    }

    [HttpPost]
    public async Task<ActionResult<DisciplinaDto>> Criar(DisciplinaUpsertRequest request)
    {
        var nomeNormalizado = request.Nome.Trim();

        var existente = await _context.Disciplinas
            .FirstOrDefaultAsync(d => d.Nome.ToLower() == nomeNormalizado.ToLower());
        if (existente is not null)
            return Ok(new DisciplinaDto(existente.Id, existente.Nome));

        var disciplina = new Disciplina { Nome = nomeNormalizado };
        _context.Disciplinas.Add(disciplina);
        await _context.SaveChangesAsync();

        return Ok(new DisciplinaDto(disciplina.Id, disciplina.Nome));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "administrador")]
    public async Task<IActionResult> Remover(int id)
    {
        var disciplina = await _context.Disciplinas.FirstOrDefaultAsync(d => d.Id == id);
        if (disciplina is null) return NotFound();

        _context.Disciplinas.Remove(disciplina);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
