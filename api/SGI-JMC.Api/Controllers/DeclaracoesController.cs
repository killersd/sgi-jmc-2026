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
public class DeclaracoesController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;

    public DeclaracoesController(AppDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    // GET api/declaracoes/buscar-aluno/MAT-2026-0001
    // Usado pela tela de Declarações: o usuário digita o código do aluno,
    // o front chama esse endpoint e exibe os dados antes de emitir o PDF.
    [HttpGet("buscar-aluno/{codigoSeed}")]
    public async Task<ActionResult<AlunoParaDeclaracaoDto>> BuscarAlunoPorCodigo(string codigoSeed)
    {
        var aluno = await _context.Alunos.AsNoTracking()
            .FirstOrDefaultAsync(a => a.CodigoSeed == codigoSeed);

        if (aluno is null)
            return NotFound(new { mensagem = "Nenhum aluno encontrado com esse código." });

        return Ok(new AlunoParaDeclaracaoDto(
            aluno.Id, aluno.Nome, aluno.Pai, aluno.Mae, aluno.DataNascimento,
            aluno.CodigoSeed, aluno.AnoLetivo, aluno.AnoSerie, aluno.Turma, aluno.NumeroDoNis));
    }

    // GET api/declaracoes/historico?codigoSeed=MAT-2026-0001
    [HttpGet("historico")]
    public async Task<ActionResult<IEnumerable<DeclaracaoEmitidaDto>>> Historico([FromQuery] string? codigoSeed)
    {
        var query = _context.Declaracoes.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(codigoSeed))
            query = query.Where(d => d.CodigoSeed == codigoSeed);

        var itens = await query
            .OrderByDescending(d => d.DataDeEmissao)
            .Select(d => new DeclaracaoEmitidaDto(d.Id, d.NomeAluno, d.NumeroDeclaracao, d.CodigoAutenticacao, d.DataDeEmissao))
            .ToListAsync();

        return Ok(itens);
    }

    // POST api/declaracoes/frequencia
    // Gera o PDF da declaração de frequência já preenchido com os dados do aluno
    // (mesmo layout/textos do sistema original) e devolve o arquivo pronto para download.
    [HttpPost("frequencia")]
    public async Task<IActionResult> GerarDeclaracaoFrequencia(GerarDeclaracaoRequest request)
    {
        var aluno = await _context.Alunos.AsNoTracking()
            .FirstOrDefaultAsync(a => a.CodigoSeed == request.CodigoSeed);

        if (aluno is null)
            return NotFound(new { mensagem = "Nenhum aluno encontrado com esse código." });

        if (aluno.AnoSerie is null || string.IsNullOrWhiteSpace(aluno.Turma))
            return BadRequest(new { mensagem = "Este aluno não possui ano/série ou turma cadastrados." });

        var numeroDeclaracao = new Random().Next().GetHashCode();
        var codigoAutenticacao = numeroDeclaracao.ToString("x");
        var emitidoEm = DateTime.UtcNow;

        var declaracao = new Declaracao
        {
            NomeAluno = aluno.Nome,
            NomePai = aluno.Pai,
            NomeMae = aluno.Mae,
            DataNascimento = aluno.DataNascimento,
            AnoLetivo = aluno.AnoLetivo,
            AnoSerie = aluno.AnoSerie.Value,
            Turma = aluno.Turma!,
            NumeroDoNis = aluno.NumeroDoNis,
            CodigoSeed = aluno.CodigoSeed,
            QtdFaltas = request.QtdFaltas,
            NumeroDeclaracao = numeroDeclaracao,
            CodigoAutenticacao = codigoAutenticacao,
            DataDeEmissao = emitidoEm,
            EmitidoPor = User.FindFirst("nomeCompleto")?.Value ?? User.Identity?.Name
        };

        var pdfBytes = GerarPdf(declaracao);

        _context.Declaracoes.Add(declaracao);
        await _context.SaveChangesAsync();

        var nomeArquivo = $"Declaracao {aluno.Nome}.pdf";
        return File(pdfBytes, "application/pdf", nomeArquivo);
    }

    // Réplica do layout gerado em SGI-JMC/Controllers/DeclaracaoController.cs (gerarDeclaracao)
    private byte[] GerarPdf(Declaracao declaracao)
    {
        float porcentagemDeFaltas = 100 - ((declaracao.QtdFaltas * 100) / 1000f);

        using var doc = new PdfSharpCore.Pdf.PdfDocument();
        var page = doc.AddPage();
        page.Size = PdfSharpCore.PageSize.A4;
        page.TrimMargins.Right = 50;
        page.TrimMargins.Left = 50;
        page.Orientation = PdfSharpCore.PageOrientation.Portrait;

        var graphics = XGraphics.FromPdfPage(page);
        var corFonte = XBrushes.Black;
        var textFormatter = new XTextFormatter(graphics);
        var fonteDescricao = new XFont("Calibri", 14);
        var fonteTitulo = new XFont("Calibri", 17, XFontStyle.Bold);
        var fonteDetalhes = new XFont("Calibri", 10);
        var fonteRodape = new XFont("Calibri", 7);

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
        textFormatter.DrawString("DECLARAÇÃO", fonteTitulo, corFonte, new XRect(0, 200, page.Width, page.Height));

        var dataNascString = declaracao.DataNascimento.ToString("dd/MM/yyyy");
        var numeroDoNis = string.IsNullOrWhiteSpace(declaracao.NumeroDoNis)
            ? "Não encontrado em nossos registros!"
            : declaracao.NumeroDoNis;

        textFormatter.Alignment = XParagraphAlignment.Justify;
        var filiacao = string.IsNullOrWhiteSpace(declaracao.NomePai)
            ? $"filho(a) de {declaracao.NomeMae.ToUpper()}, "
            : $"filho(a) de {declaracao.NomeMae.ToUpper()} e {declaracao.NomePai.ToUpper()}, ";

        textFormatter.DrawString(
            $"Declaro para os devidos fins que o aluno(a) {declaracao.NomeAluno.ToUpper()}, nascido(a) em {dataNascString}, {filiacao}" +
            $"no ano letivo de {declaracao.AnoLetivo}, encontra-se matriculado(a) nesta Unidade de Ensino no {declaracao.AnoSerie}º ano, " +
            $"turma \"{declaracao.Turma.ToUpper()}\" e da carga horária anual (833 horas), possui frequência de {porcentagemDeFaltas}% nesta data.",
            fonteDescricao, corFonte, new XRect(0, 270, page.Width, page.Height));

        textFormatter.DrawString($"NIS: {numeroDoNis}", fonteDescricao, corFonte, new XRect(0, 400, page.Width, page.Height));
        textFormatter.DrawString($"Código do aluno: {declaracao.CodigoSeed}", fonteDescricao, corFonte, new XRect(0, 415, page.Width, page.Height));
        textFormatter.DrawString(
            "Observação: Esta declaração não contém emendas nem rasuras e é válida por um período de 30 dias ",
            fonteDescricao, corFonte, new XRect(0, 730, page.Width, page.Height));

        textFormatter.Alignment = XParagraphAlignment.Center;
        textFormatter.DrawString(new string('_', 60), fonteDetalhes, corFonte, new XRect(0, 470, page.Width, page.Height));
        textFormatter.DrawString("Equipe Diretiva", fonteDescricao, corFonte, new XRect(0, 480, page.Width, page.Height));

        textFormatter.DrawString($"Número do documento: {declaracao.NumeroDeclaracao}", fonteDescricao, corFonte, new XRect(0, 600, page.Width, page.Height));
        textFormatter.DrawString($"Código de verificação: {declaracao.CodigoAutenticacao}", fonteDescricao, corFonte, new XRect(0, 613, page.Width, page.Height));
        textFormatter.DrawString(
            "Para verificar a autenticidade deste documento acesse o portal do SGI-JMC, preencha os dados " +
            "\"Número do documento\" e \"Código de verificação\" com os códigos acima depois clique no botão \"Verificar autenticidade\" ",
            fonteDetalhes, corFonte, new XRect(0, 635, page.Width, page.Height));

        textFormatter.Alignment = XParagraphAlignment.Center;
        textFormatter.DrawString($"Declaração emitida em {declaracao.DataDeEmissao:dd/MM/yyyy}", fonteDetalhes, corFonte, new XRect(0, 780, page.Width, page.Height));

        textFormatter.Alignment = XParagraphAlignment.Justify;
        textFormatter.DrawString(new string('_', 89), fonteDescricao, corFonte, new XRect(0, 750, page.Width, page.Height));
        textFormatter.DrawString("Esta declaração foi gerada através do SGI da Escola Estadual João de Mattos Carvalho ", fonteRodape, corFonte, new XRect(0, 810, page.Width, page.Height));

        textFormatter.Alignment = XParagraphAlignment.Right;
        textFormatter.DrawString("Contato: eejmc.seed@seduc.se.gov.br ", fonteRodape, corFonte, new XRect(0, 810, page.Width, page.Height));
        textFormatter.DrawString("SGI-Sistema de Gerenciamento Interno - EEJMC ", fonteRodape, corFonte, new XRect(0, 30, page.Width, page.Height));
        textFormatter.DrawString($"Usuário: {declaracao.EmitidoPor}", fonteRodape, corFonte, new XRect(0, 40, page.Width, page.Height));

        using var stream = new MemoryStream();
        doc.Save(stream, false);
        return stream.ToArray();
    }
}