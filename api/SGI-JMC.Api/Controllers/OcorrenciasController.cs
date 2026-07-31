using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PdfSharpCore.Drawing;
using PdfSharpCore.Drawing.Layout;
using SGI_JMC.Api.Data;
using SGI_JMC.Api.DTOs;
using SGI_JMC.Api.Models;

namespace SGI_JMC.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "usuario,administrador")]
public class OcorrenciasController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;

    public OcorrenciasController(AppDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    // GET api/ocorrencias/historico?codigoSeed=MAT-2026-0001
    [HttpGet("historico")]
    public async Task<ActionResult<IEnumerable<OcorrenciaEmitidaDto>>> Historico([FromQuery] string? codigoSeed)
    {
        var advertencias = _context.Advertencias.AsNoTracking().AsQueryable();
        var suspensoes = _context.Suspensoes.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(codigoSeed))
        {
            advertencias = advertencias.Where(a => a.CodigoSeed == codigoSeed);
            suspensoes = suspensoes.Where(s => s.CodigoSeed == codigoSeed);
        }

        var itensAdvertencia = await advertencias
            .Select(a => new OcorrenciaEmitidaDto(a.Id, a.NomeAluno, "Advertência", a.DataDeEmissao))
            .ToListAsync();

        var itensSuspensao = await suspensoes
            .Select(s => new OcorrenciaEmitidaDto(s.Id, s.NomeAluno, "Suspensão", s.DataDeEmissao))
            .ToListAsync();

        var todos = itensAdvertencia.Concat(itensSuspensao).OrderByDescending(o => o.DataDeEmissao);
        return Ok(todos);
    }

    // POST api/ocorrencias/advertencia
    [HttpPost("advertencia")]
    public async Task<IActionResult> GerarAdvertencia(GerarAdvertenciaRequest request)
    {
        var aluno = await _context.Alunos.AsNoTracking().FirstOrDefaultAsync(a => a.CodigoSeed == request.CodigoSeed);
        if (aluno is null)
            return NotFound(new { mensagem = "Nenhum aluno encontrado com esse código." });

        var advertencia = new Advertencia
        {
            NomeAluno = aluno.Nome,
            NomePai = aluno.Pai,
            NomeMae = aluno.Mae,
            DataNascimento = aluno.DataNascimento,
            AnoSerie = aluno.AnoSerie?.ToString() ?? "",
            Turma = aluno.Turma ?? "",
            Turno = request.Turno,
            CodigoSeed = aluno.CodigoSeed,
            DescricaoDoFato = request.DescricaoDoFato,
            Numero = request.Numero,
            DataDeEmissao = DateTime.UtcNow,
            EmitidoPor = User.FindFirst("nomeCompleto")?.Value ?? User.Identity?.Name
        };

        var pdfBytes = GerarPdfAdvertencia(advertencia);

        _context.Advertencias.Add(advertencia);
        await _context.SaveChangesAsync();

        return File(pdfBytes, "application/pdf", $"Advertencia {aluno.Nome}.pdf");
    }

    // POST api/ocorrencias/suspensao
    [HttpPost("suspensao")]
    public async Task<IActionResult> GerarSuspensao(GerarSuspensaoRequest request)
    {
        var aluno = await _context.Alunos.AsNoTracking().FirstOrDefaultAsync(a => a.CodigoSeed == request.CodigoSeed);
        if (aluno is null)
            return NotFound(new { mensagem = "Nenhum aluno encontrado com esse código." });

        var suspensao = new Suspensao
        {
            NomeAluno = aluno.Nome,
            NomePai = aluno.Pai,
            NomeMae = aluno.Mae,
            DataNascimento = aluno.DataNascimento,
            AnoSerie = aluno.AnoSerie?.ToString() ?? "",
            Turma = aluno.Turma ?? "",
            Turno = request.Turno,
            CodigoSeed = aluno.CodigoSeed,
            DescricaoDoFato = request.DescricaoDoFato,
            Dias = request.Dias,
            NumeroSuspensao = request.NumeroSuspensao,
            DataDeEmissao = DateTime.UtcNow,
            EmitidoPor = User.FindFirst("nomeCompleto")?.Value ?? User.Identity?.Name
        };

        var pdfBytes = GerarPdfSuspensao(suspensao);

        _context.Suspensoes.Add(suspensao);
        await _context.SaveChangesAsync();

        return File(pdfBytes, "application/pdf", $"Suspensao {aluno.Nome}.pdf");
    }

    private (XGraphics graphics, PdfSharpCore.Pdf.PdfPage page, PdfSharpCore.Pdf.PdfDocument doc) NovaPaginaComCabecalho(
        string titulo, out XTextFormatter textFormatter, out XFont fonteDescricao, out XFont fonteRodape, out XBrush corFonte)
    {
        var doc = new PdfSharpCore.Pdf.PdfDocument();
        var page = doc.AddPage();
        page.Size = PdfSharpCore.PageSize.A4;
        page.TrimMargins.Right = 50;
        page.TrimMargins.Left = 50;
        page.Orientation = PdfSharpCore.PageOrientation.Portrait;

        var graphics = XGraphics.FromPdfPage(page);
        corFonte = XBrushes.Black;
        textFormatter = new XTextFormatter(graphics);
        fonteDescricao = new XFont("Calibri", 14);
        var fonteTitulo = new XFont("Calibri", 17, XFontStyle.Bold);
        fonteRodape = new XFont("Calibri", 7);

        var imagensDir = Path.Combine(_env.ContentRootPath, "wwwroot", "Imagens");
        var imgBrasao = XImage.FromFile(Path.Combine(imagensDir, "BrasaoEstado.png"));
        var imgEscudo = XImage.FromFile(Path.Combine(imagensDir, "Escudo.png"));
        var imgLogo = XImage.FromFile(Path.Combine(imagensDir, "SGI.jpg"));

        textFormatter.Alignment = XParagraphAlignment.Left;
        graphics.DrawImage(imgBrasao, 0, 30, 50, 75);
        graphics.DrawImage(imgEscudo, 75, 280, 450, 450);
        graphics.DrawImage(imgLogo, 480, 60, 120, 50);

        textFormatter.DrawString("GOVERNO DO ESTADO DE SERGIPE", fonteDescricao, corFonte, new XRect(55, 30, page.Width, page.Height));
        textFormatter.DrawString("SECRETARIA DE ESTADO DA EDUCAÇÃO E DA CULTURA", fonteDescricao, corFonte, new XRect(55, 45, page.Width, page.Height));
        textFormatter.DrawString("ESCOLA ESTADUAL JOÃO DE MATTOS CARVALHO", fonteDescricao, corFonte, new XRect(55, 60, page.Width, page.Height));
        textFormatter.DrawString("CNPJ: 01.902.194/0001-83", fonteDescricao, corFonte, new XRect(55, 75, page.Width, page.Height));
        textFormatter.DrawString("PRAÇA ABEL JACÓ DOS SANTOS, Nº 892, CENTRO, SIMÃO DIAS - SE", fonteDescricao, corFonte, new XRect(55, 90, page.Width, page.Height));
        textFormatter.DrawString(new string('_', 89), fonteDescricao, corFonte, new XRect(0, 100, page.Width, page.Height));

        textFormatter.Alignment = XParagraphAlignment.Center;
        textFormatter.DrawString(titulo, fonteTitulo, corFonte, new XRect(0, 200, page.Width, page.Height));

        return (graphics, page, doc);
    }

    private byte[] GerarPdfAdvertencia(Advertencia advertencia)
    {
        var (graphics, page, doc) = NovaPaginaComCabecalho(
            "ADVERTÊNCIA ESCRITA", out var textFormatter, out var fonteDescricao, out var fonteRodape, out var corFonte);

        var dataNascString = advertencia.DataNascimento.ToString("dd/MM/yyyy");
        var filiacao = string.IsNullOrWhiteSpace(advertencia.NomePai)
            ? $"filho(a) de {advertencia.NomeMae.ToUpper()}, "
            : $"filho(a) de {advertencia.NomeMae.ToUpper()} e {advertencia.NomePai.ToUpper()}, ";

        textFormatter.Alignment = XParagraphAlignment.Justify;
        textFormatter.DrawString(
            $"Comunicamos que o(a) aluno(a) {advertencia.NomeAluno.ToUpper()}, nascido(a) em {dataNascString}, {filiacao}" +
            $"matriculado(a) nesta Unidade de Ensino no {advertencia.AnoSerie}º ano, turma \"{advertencia.Turma.ToUpper()}\", " +
            $"turno {advertencia.Turno}, recebeu ADVERTÊNCIA ESCRITA Nº {advertencia.Numero}, pelo(s) motivo(s) a seguir descrito(s):",
            fonteDescricao, corFonte, new XRect(0, 270, page.Width, page.Height));

        textFormatter.DrawString(advertencia.DescricaoDoFato, fonteDescricao, corFonte, new XRect(0, 340, page.Width, page.Height));

        textFormatter.DrawString($"Código do aluno: {advertencia.CodigoSeed}", fonteDescricao, corFonte, new XRect(0, 470, page.Width, page.Height));

        textFormatter.Alignment = XParagraphAlignment.Center;
        textFormatter.DrawString(new string('_', 60), fonteDescricao, corFonte, new XRect(0, 560, page.Width, page.Height));
        textFormatter.DrawString("Equipe Diretiva", fonteDescricao, corFonte, new XRect(0, 570, page.Width, page.Height));

        textFormatter.DrawString($"Advertência emitida em {advertencia.DataDeEmissao:dd/MM/yyyy}", fonteRodape, corFonte, new XRect(0, 780, page.Width, page.Height));
        textFormatter.Alignment = XParagraphAlignment.Justify;
        textFormatter.DrawString(new string('_', 89), fonteDescricao, corFonte, new XRect(0, 750, page.Width, page.Height));
        textFormatter.DrawString("Este documento foi gerado através do SGI da Escola Estadual João de Mattos Carvalho ", fonteRodape, corFonte, new XRect(0, 810, page.Width, page.Height));
        textFormatter.Alignment = XParagraphAlignment.Right;
        textFormatter.DrawString("Contato: eejmc.seed@seduc.se.gov.br ", fonteRodape, corFonte, new XRect(0, 810, page.Width, page.Height));
        textFormatter.DrawString("SGI-Sistema de Gerenciamento Interno - EEJMC ", fonteRodape, corFonte, new XRect(0, 30, page.Width, page.Height));
        textFormatter.DrawString($"Usuário: {advertencia.EmitidoPor}", fonteRodape, corFonte, new XRect(0, 40, page.Width, page.Height));

        using var stream = new MemoryStream();
        doc.Save(stream, false);
        return stream.ToArray();
    }

    private byte[] GerarPdfSuspensao(Suspensao suspensao)
    {
        var (graphics, page, doc) = NovaPaginaComCabecalho(
            "SUSPENSÃO", out var textFormatter, out var fonteDescricao, out var fonteRodape, out var corFonte);

        var dataNascString = suspensao.DataNascimento.ToString("dd/MM/yyyy");
        var filiacao = string.IsNullOrWhiteSpace(suspensao.NomePai)
            ? $"filho(a) de {suspensao.NomeMae.ToUpper()}, "
            : $"filho(a) de {suspensao.NomeMae.ToUpper()} e {suspensao.NomePai.ToUpper()}, ";

        textFormatter.Alignment = XParagraphAlignment.Justify;
        textFormatter.DrawString(
            $"Comunicamos que o(a) aluno(a) {suspensao.NomeAluno.ToUpper()}, nascido(a) em {dataNascString}, {filiacao}" +
            $"matriculado(a) nesta Unidade de Ensino no {suspensao.AnoSerie}º ano, turma \"{suspensao.Turma.ToUpper()}\", " +
            $"turno {suspensao.Turno}, está SUSPENSO(A) por {suspensao.Dias} dia(s), referente à SUSPENSÃO Nº {suspensao.NumeroSuspensao}, " +
            "pelo(s) motivo(s) a seguir descrito(s):",
            fonteDescricao, corFonte, new XRect(0, 270, page.Width, page.Height));

        textFormatter.DrawString(suspensao.DescricaoDoFato, fonteDescricao, corFonte, new XRect(0, 350, page.Width, page.Height));

        textFormatter.DrawString($"Código do aluno: {suspensao.CodigoSeed}", fonteDescricao, corFonte, new XRect(0, 470, page.Width, page.Height));

        textFormatter.Alignment = XParagraphAlignment.Center;
        textFormatter.DrawString(new string('_', 60), fonteDescricao, corFonte, new XRect(0, 560, page.Width, page.Height));
        textFormatter.DrawString("Equipe Diretiva", fonteDescricao, corFonte, new XRect(0, 570, page.Width, page.Height));

        textFormatter.DrawString($"Suspensão emitida em {suspensao.DataDeEmissao:dd/MM/yyyy}", fonteRodape, corFonte, new XRect(0, 780, page.Width, page.Height));
        textFormatter.Alignment = XParagraphAlignment.Justify;
        textFormatter.DrawString(new string('_', 89), fonteDescricao, corFonte, new XRect(0, 750, page.Width, page.Height));
        textFormatter.DrawString("Este documento foi gerado através do SGI da Escola Estadual João de Mattos Carvalho ", fonteRodape, corFonte, new XRect(0, 810, page.Width, page.Height));
        textFormatter.Alignment = XParagraphAlignment.Right;
        textFormatter.DrawString("Contato: eejmc.seed@seduc.se.gov.br ", fonteRodape, corFonte, new XRect(0, 810, page.Width, page.Height));
        textFormatter.DrawString("SGI-Sistema de Gerenciamento Interno - EEJMC ", fonteRodape, corFonte, new XRect(0, 30, page.Width, page.Height));
        textFormatter.DrawString($"Usuário: {suspensao.EmitidoPor}", fonteRodape, corFonte, new XRect(0, 40, page.Width, page.Height));

        using var stream = new MemoryStream();
        doc.Save(stream, false);
        return stream.ToArray();
    }
}
