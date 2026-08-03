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
[RequerModulo(Modulos.Alunos)]
public class AlunosController : ControllerBase
{
    private readonly AppDbContext _context;

    public AlunosController(AppDbContext context)
    {
        _context = context;
    }

    private static AlunoDto ParaDto(Aluno a) => new(
        a.Id, a.Nome, a.Pai, a.Mae, a.DataNascimento, a.Endereco, a.Telefone,
        a.CodigoSeed, a.AnoLetivo, a.AnoSerie, a.Turma, a.NumeroDoNis,
        a.CorrecaoDeFluxo, a.Transferido, a.UrlFoto);

    // O Postgres exige DateTime com Kind=Utc para colunas "timestamp with time zone".
    // Datas vindas do formulário (input type="date") chegam com Kind=Unspecified.
    private static DateTime ComoUtc(DateTime data) =>
        DateTime.SpecifyKind(data, DateTimeKind.Utc);

    // GET api/alunos?anoSerie=6&turma=A&busca=maria&transferido=false&page=1&pageSize=20
    [HttpGet]
    public async Task<ActionResult<PagedResult<AlunoDto>>> Listar(
        [FromQuery] int? anoSerie,
        [FromQuery] string? turma,
        [FromQuery] string? busca,
        [FromQuery] bool? transferido,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = _context.Alunos.AsNoTracking().AsQueryable();

        if (anoSerie.HasValue)
            query = query.Where(a => a.AnoSerie == anoSerie);
        if (!string.IsNullOrWhiteSpace(turma))
            query = query.Where(a => a.Turma == turma);
        if (transferido.HasValue)
            query = query.Where(a => a.Transferido == transferido);
        if (!string.IsNullOrWhiteSpace(busca))
            query = query.Where(a => a.Nome.Contains(busca));

        var total = await query.CountAsync();
        var itens = await query
            .OrderBy(a => a.Nome)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(a => ParaDto(a))
            .ToListAsync();

        return Ok(new PagedResult<AlunoDto>(itens, total, page, pageSize));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AlunoDto>> ObterPorId(int id)
    {
        var aluno = await _context.Alunos.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
        return aluno is null ? NotFound() : Ok(ParaDto(aluno));
    }

    [HttpPost]
    public async Task<ActionResult<AlunoDto>> Criar(AlunoUpsertRequest request)
    {
        if (await _context.Alunos.AnyAsync(a => a.CodigoSeed == request.CodigoSeed))
            return Conflict(new { mensagem = "Já existe um aluno cadastrado com este código." });

        var aluno = new Aluno
        {
            Nome = request.Nome,
            Pai = request.Pai,
            Mae = request.Mae,
            DataNascimento = ComoUtc(request.DataNascimento),
            Endereco = request.Endereco,
            Telefone = request.Telefone,
            CodigoSeed = request.CodigoSeed,
            AnoLetivo = request.AnoLetivo,
            AnoSerie = request.AnoSerie,
            Turma = request.Turma,
            NumeroDoNis = request.NumeroDoNis,
            CorrecaoDeFluxo = request.CorrecaoDeFluxo,
            Transferido = request.Transferido,
            DataDeEmissao = DateTime.UtcNow
        };

        _context.Alunos.Add(aluno);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(ObterPorId), new { id = aluno.Id }, ParaDto(aluno));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<AlunoDto>> Atualizar(int id, AlunoUpsertRequest request)
    {
        var aluno = await _context.Alunos.FirstOrDefaultAsync(a => a.Id == id);
        if (aluno is null) return NotFound();

        if (aluno.CodigoSeed != request.CodigoSeed &&
            await _context.Alunos.AnyAsync(a => a.CodigoSeed == request.CodigoSeed))
            return Conflict(new { mensagem = "Já existe um aluno cadastrado com este código." });

        aluno.Nome = request.Nome;
        aluno.Pai = request.Pai;
        aluno.Mae = request.Mae;
        aluno.DataNascimento = ComoUtc(request.DataNascimento);
        aluno.Endereco = request.Endereco;
        aluno.Telefone = request.Telefone;
        aluno.CodigoSeed = request.CodigoSeed;
        aluno.AnoLetivo = request.AnoLetivo;
        aluno.AnoSerie = request.AnoSerie;
        aluno.Turma = request.Turma;
        aluno.NumeroDoNis = request.NumeroDoNis;
        aluno.CorrecaoDeFluxo = request.CorrecaoDeFluxo;
        aluno.Transferido = request.Transferido;

        await _context.SaveChangesAsync();
        return Ok(ParaDto(aluno));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "administrador")]
    public async Task<IActionResult> Remover(int id)
    {
        var aluno = await _context.Alunos.FirstOrDefaultAsync(a => a.Id == id);
        if (aluno is null) return NotFound();

        _context.Alunos.Remove(aluno);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
