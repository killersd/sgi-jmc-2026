using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SGI_JMC.Api.Authorization;
using PdfSharpCore.Drawing;
using SGI_JMC.Api.Data;
using SGI_JMC.Api.DTOs;
using SGI_JMC.Api.Models;

namespace SGI_JMC.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[RequerModulo(Modulos.Horarios)]
public class HorariosController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public HorariosController(AppDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    private static HorarioProfessorDto ParaDto(HorarioProfessor h)
    {
        var grade = string.IsNullOrWhiteSpace(h.GradeJson)
            ? new GradeHorarioDto()
            : JsonSerializer.Deserialize<GradeHorarioDto>(h.GradeJson, JsonOptions) ?? new GradeHorarioDto();

        var disciplinas = h.Professor.ProfessorDisciplinas
            .Select(pd => pd.Disciplina.Nome)
            .OrderBy(nome => nome)
            .ToList();

        return new HorarioProfessorDto(
            h.Id, h.ProfessorId, h.Professor.Nome, h.Professor.CPF, h.Professor.Cargo,
            h.Professor.CargaHorariaSemanal, disciplinas, grade);
    }

    private IQueryable<HorarioProfessor> QueryComProfessor() =>
        _context.HorariosProfessor
            .Include(h => h.Professor).ThenInclude(p => p.ProfessorDisciplinas).ThenInclude(pd => pd.Disciplina);

    // GET api/horarios?busca=maria
    [HttpGet]
    public async Task<ActionResult<IEnumerable<HorarioProfessorDto>>> Listar([FromQuery] string? busca)
    {
        var query = QueryComProfessor().AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(busca))
            query = query.Where(h => h.Professor.Nome.Contains(busca));

        var itens = await query.OrderBy(h => h.Professor.Nome).ToListAsync();
        return Ok(itens.Select(ParaDto));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<HorarioProfessorDto>> ObterPorId(int id)
    {
        var horario = await QueryComProfessor().AsNoTracking().FirstOrDefaultAsync(h => h.Id == id);
        return horario is null ? NotFound() : Ok(ParaDto(horario));
    }

    [HttpPost]
    public async Task<ActionResult<HorarioProfessorDto>> Criar(HorarioProfessorUpsertRequest request)
    {
        var professorExiste = await _context.Professores.AnyAsync(p => p.Id == request.ProfessorId);
        if (!professorExiste)
            return BadRequest(new { mensagem = "Professor não encontrado." });

        if (await _context.HorariosProfessor.AnyAsync(h => h.ProfessorId == request.ProfessorId))
            return Conflict(new { mensagem = "Este professor já possui um horário cadastrado. Edite o horário existente." });

        var horario = new HorarioProfessor
        {
            ProfessorId = request.ProfessorId,
            GradeJson = JsonSerializer.Serialize(request.Grade, JsonOptions)
        };

        _context.HorariosProfessor.Add(horario);
        await _context.SaveChangesAsync();

        return await ObterPorId(horario.Id);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<HorarioProfessorDto>> Atualizar(int id, HorarioProfessorUpsertRequest request)
    {
        var horario = await _context.HorariosProfessor.FirstOrDefaultAsync(h => h.Id == id);
        if (horario is null) return NotFound();

        var professorExiste = await _context.Professores.AnyAsync(p => p.Id == request.ProfessorId);
        if (!professorExiste)
            return BadRequest(new { mensagem = "Professor não encontrado." });

        horario.ProfessorId = request.ProfessorId;
        horario.GradeJson = JsonSerializer.Serialize(request.Grade, JsonOptions);

        await _context.SaveChangesAsync();
        return await ObterPorId(id);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "administrador")]
    public async Task<IActionResult> Remover(int id)
    {
        var horario = await _context.HorariosProfessor.FirstOrDefaultAsync(h => h.Id == id);
        if (horario is null) return NotFound();

        _context.HorariosProfessor.Remove(horario);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // GET api/horarios/{id}/pdf
    [HttpGet("{id:int}/pdf")]
    public async Task<IActionResult> ImprimirPdf(int id)
    {
        var horario = await QueryComProfessor().AsNoTracking().FirstOrDefaultAsync(h => h.Id == id);
        if (horario is null) return NotFound();

        var grade = string.IsNullOrWhiteSpace(horario.GradeJson)
            ? new GradeHorarioDto()
            : JsonSerializer.Deserialize<GradeHorarioDto>(horario.GradeJson, JsonOptions) ?? new GradeHorarioDto();

        var turmas = await _context.Turmas.AsNoTracking().ToDictionaryAsync(t => t.Id, t => t.Nome);
        var disciplinas = await _context.Disciplinas.AsNoTracking().ToDictionaryAsync(d => d.Id, d => d.Nome);

        var pdfBytes = GerarPdf(horario.Professor, grade, turmas, disciplinas);
        var nomeArquivo = $"Horario {horario.Professor.Nome}.pdf";
        return File(pdfBytes, "application/pdf", nomeArquivo);
    }

    private byte[] GerarPdf(
        Professor professor,
        GradeHorarioDto grade,
        Dictionary<int, string> turmas,
        Dictionary<int, string> disciplinas)
    {
        using var doc = new PdfSharpCore.Pdf.PdfDocument();
        var page = doc.AddPage();
        page.Size = PdfSharpCore.PageSize.A4;
        page.Orientation = PdfSharpCore.PageOrientation.Portrait;

        var graphics = XGraphics.FromPdfPage(page);
        var corFonte = XBrushes.Black;
        var corLinha = XPens.Gray;
        var fonteTitulo = new XFont("Calibri", 15, XFontStyle.Bold);
        var fonteDescricao = new XFont("Calibri", 11);
        var fonteCabecalho = new XFont("Calibri", 8, XFontStyle.Bold);
        var fonteCelula = new XFont("Calibri", 7.5);

        var margem = 40.0;
        var larguraUtil = page.Width - margem * 2;
        double y = 40;

        graphics.DrawString("ESCOLA ESTADUAL JOÃO DE MATTOS CARVALHO", fonteDescricao, corFonte, new XRect(margem, y, larguraUtil, 20), XStringFormats.TopLeft);
        y += 22;
        graphics.DrawString("Horário de Aulas", fonteTitulo, corFonte, new XRect(margem, y, larguraUtil, 24), XStringFormats.TopLeft);
        y += 30;

        graphics.DrawString($"Professor(a): {professor.Nome}", fonteDescricao, corFonte, new XRect(margem, y, larguraUtil, 18), XStringFormats.TopLeft);
        y += 18;
        graphics.DrawString($"Cargo: {professor.Cargo}    Carga horária semanal: {professor.CargaHorariaSemanal}h", fonteDescricao, corFonte, new XRect(margem, y, larguraUtil, 18), XStringFormats.TopLeft);
        y += 18;
        var nomesDisciplinas = string.Join(", ", professor.ProfessorDisciplinas.Select(pd => pd.Disciplina.Nome));
        graphics.DrawString($"Disciplinas: {nomesDisciplinas}", fonteDescricao, corFonte, new XRect(margem, y, larguraUtil, 18), XStringFormats.TopLeft);
        y += 30;

        y = DesenharTabelaTurno(graphics, "Manhã", grade.Manha, margem, y, larguraUtil, turmas, disciplinas, fonteCabecalho, fonteCelula, corFonte, corLinha);
        y += 24;
        DesenharTabelaTurno(graphics, "Tarde", grade.Tarde, margem, y, larguraUtil, turmas, disciplinas, fonteCabecalho, fonteCelula, corFonte, corLinha);

        using var stream = new MemoryStream();
        doc.Save(stream, false);
        return stream.ToArray();
    }

    private static double DesenharTabelaTurno(
        XGraphics graphics, string titulo, GradeTurnoDto turno,
        double x0, double y0, double largura,
        Dictionary<int, string> turmas, Dictionary<int, string> disciplinas,
        XFont fonteCabecalho, XFont fonteCelula, XBrush corFonte, XPen corLinha)
    {
        var fonteTituloTurno = new XFont("Calibri", 11, XFontStyle.Bold);
        graphics.DrawString(titulo, fonteTituloTurno, corFonte, new XRect(x0, y0, largura, 16), XStringFormats.TopLeft);
        y0 += 20;

        var dias = new[] { "Segunda", "Terça", "Quarta", "Quinta", "Sexta", "Sábado" };
        var colunas = new[] { turno.Seg, turno.Ter, turno.Qua, turno.Qui, turno.Sex, turno.Sab };

        var colPeriodo = 30.0;
        var colDia = (largura - colPeriodo) / dias.Length;
        var linhaAltura = 30.0;
        var alturaCabecalho = 20.0;

        // Cabeçalho
        graphics.DrawRectangle(corLinha, x0, y0, colPeriodo, alturaCabecalho);
        for (int c = 0; c < dias.Length; c++)
        {
            var cx = x0 + colPeriodo + c * colDia;
            graphics.DrawRectangle(corLinha, cx, y0, colDia, alturaCabecalho);
            graphics.DrawString(dias[c], fonteCabecalho, corFonte, new XRect(cx, y0, colDia, alturaCabecalho), XStringFormats.Center);
        }
        y0 += alturaCabecalho;

        for (int linha = 0; linha < 5; linha++)
        {
            graphics.DrawRectangle(corLinha, x0, y0, colPeriodo, linhaAltura);
            graphics.DrawString($"{linha + 1}ª", fonteCabecalho, corFonte, new XRect(x0, y0, colPeriodo, linhaAltura), XStringFormats.Center);

            for (int c = 0; c < dias.Length; c++)
            {
                var cx = x0 + colPeriodo + c * colDia;
                graphics.DrawRectangle(corLinha, cx, y0, colDia, linhaAltura);

                var celula = colunas[c][linha];
                var turmaNome = celula?.TurmaId is int tId && turmas.TryGetValue(tId, out var tNome) ? tNome : null;
                var disciplinaNome = celula?.DisciplinaId is int dId && disciplinas.TryGetValue(dId, out var dNome) ? dNome : null;

                if (turmaNome is not null || disciplinaNome is not null)
                {
                    graphics.DrawString(turmaNome ?? "—", fonteCelula, corFonte, new XRect(cx, y0 + 2, colDia, 13), XStringFormats.Center);
                    graphics.DrawString(disciplinaNome ?? "—", fonteCelula, corFonte, new XRect(cx, y0 + 15, colDia, 13), XStringFormats.Center);
                }
            }
            y0 += linhaAltura;
        }

        return y0;
    }
}
