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
[RequerModulo(Modulos.Turmas)]
public class TurmasController : ControllerBase
{
    private readonly AppDbContext _context;

    public TurmasController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TurmaDto>>> Listar()
    {
        var itens = await _context.Turmas.AsNoTracking()
            .OrderBy(t => t.Nome)
            .Select(t => new TurmaDto(t.Id, t.Nome))
            .ToListAsync();

        return Ok(itens);
    }

    [HttpPost]
    public async Task<ActionResult<TurmaDto>> Criar(TurmaUpsertRequest request)
    {
        var nomeNormalizado = request.Nome.Trim();

        var existente = await _context.Turmas
            .FirstOrDefaultAsync(t => t.Nome.ToLower() == nomeNormalizado.ToLower());
        if (existente is not null)
            return Ok(new TurmaDto(existente.Id, existente.Nome));

        var turma = new Turma { Nome = nomeNormalizado };
        _context.Turmas.Add(turma);
        await _context.SaveChangesAsync();

        return Ok(new TurmaDto(turma.Id, turma.Nome));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "administrador")]
    public async Task<IActionResult> Remover(int id)
    {
        var turma = await _context.Turmas.FirstOrDefaultAsync(t => t.Id == id);
        if (turma is null) return NotFound();

        _context.Turmas.Remove(turma);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
