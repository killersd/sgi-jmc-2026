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
public class OficiosController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;

    public OficiosController(AppDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    // GET api/oficios/geral/historico
    [HttpGet("geral/historico")]
    public async Task<ActionResult<IEnumerable<OficioEmitidoDto>>> Historico()
    {
        var itens = await _context.Oficios.AsNoTracking()
            .OrderByDescending(o => o.DataEmissao)
            .Select(o => new OficioEmitidoDto(o.Id, o.NumeroOficio, o.Assunto, o.Destinatario, o.DataEmissao))
            .ToListAsync();

        return Ok(itens);
    }

    // POST api/oficios/geral
    // Gera o PDF do Ofício Geral (carta livre) já formatado, e devolve para download.
    [HttpPost("geral")]
    public async Task<IActionResult> GerarOficioGeral(GerarOficioGeralRequest request)
    {
        var oficio = new Oficio
        {
            NumeroOficio = request.NumeroOficio,
            Assunto = request.Assunto,
            Destinatario = request.Destinatario,
            CargoDoDestinatario = request.CargoDoDestinatario,
            CorpoDoOficio = request.CorpoDoOficio,
            Remetente = request.Remetente,
            Cidade = request.Cidade,
            DataEmissao = DateTime.UtcNow,
            EmitidoPor = User.FindFirst("nomeCompleto")?.Value ?? User.Identity?.Name
        };

        var pdfBytes = GerarPdf(oficio);

        _context.Oficios.Add(oficio);
        await _context.SaveChangesAsync();

        var nomeArquivo = $"Oficio nº{oficio.NumeroOficio}.pdf";
        return File(pdfBytes, "application/pdf", nomeArquivo);
    }

    // Réplica do layout gerado em SGI-JMC/Controllers/OficioGeralController.cs (gerarOficio)
    private byte[] GerarPdf(Oficio oficio)
    {
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
        var fonteDescricaoBold = new XFont("Calibri", 14, XFontStyle.Bold);
        var fonteDetalhes = new XFont("Calibri", 10);
        var fonteRodape = new XFont("Calibri", 7);

        var imagensDir = Path.Combine(_env.ContentRootPath, "wwwroot", "Imagens");
        var imgBrasao = XImage.FromFile(Path.Combine(imagensDir, "BrasaoEstado.png"));
        var imgEscudo = XImage.FromFile(Path.Combine(imagensDir, "Escudo.png"));

        // Usuário (rodapé superior)
        textFormatter.Alignment = XParagraphAlignment.Right;
        textFormatter.DrawString("SGI-Sistema de Gerenciamento Interno - EEJMC ", fonteRodape, corFonte, new XRect(0, 30, page.Width, page.Height));
        textFormatter.DrawString($"Usuário: {oficio.EmitidoPor}", fonteRodape, corFonte, new XRect(0, 40, page.Width, page.Height));

        // Cabeçalho
        textFormatter.Alignment = XParagraphAlignment.Left;
        graphics.DrawImage(imgBrasao, 0, 30, 50, 75);
        graphics.DrawImage(imgEscudo, 75, 280, 450, 450);
        textFormatter.DrawString("GOVERNO DO ESTADO DE SERGIPE", fonteDescricao, corFonte, new XRect(55, 30, page.Width, page.Height));
        textFormatter.DrawString("SECRETARIA DE ESTADO DA EDUCAÇÃO E DA CULTURA", fonteDescricao, corFonte, new XRect(55, 45, page.Width, page.Height));
        textFormatter.DrawString("ESCOLA ESTADUAL JOÃO DE MATTOS CARVALHO", fonteDescricao, corFonte, new XRect(55, 60, page.Width, page.Height));
        textFormatter.DrawString("CNPJ: 01.902.194/0001-83", fonteDescricao, corFonte, new XRect(55, 75, page.Width, page.Height));
        textFormatter.DrawString("PRAÇA ABEL JACÓ DOS SANTOS, Nº 892, CENTRO, SIMÃO DIAS - SE", fonteDescricao, corFonte, new XRect(55, 90, page.Width, page.Height));
        textFormatter.DrawString(new string('_', 89), fonteDescricao, corFonte, new XRect(0, 100, page.Width, page.Height));

        // Início
        textFormatter.Alignment = XParagraphAlignment.Left;
        textFormatter.DrawString($"Ofício nº 00{oficio.NumeroOficio}/{oficio.DataEmissao.Year}", fonteDescricao, corFonte, new XRect(0, 150, page.Width, page.Height));
        textFormatter.DrawString($"Assunto: {oficio.Assunto}.", fonteDescricao, corFonte, new XRect(0, 165, page.Width, page.Height));
        textFormatter.DrawString($"Simão Dias - Se -  {oficio.DataEmissao:dd/MM/yyyy}", fonteDescricao, corFonte, new XRect(0, 200, page.Width, page.Height));

        // Corpo do ofício
        textFormatter.DrawString($"Senhor(a) {oficio.Destinatario},", fonteDescricao, corFonte, new XRect(0, 235, page.Width, page.Height));
        textFormatter.Alignment = XParagraphAlignment.Justify;
        textFormatter.DrawString(oficio.CorpoDoOficio, fonteDescricao, corFonte, new XRect(0, 275, page.Width, page.Height));

        // Assinatura
        textFormatter.Alignment = XParagraphAlignment.Center;
        textFormatter.DrawString(new string('_', 60), fonteDetalhes, corFonte, new XRect(0, 625, page.Width, page.Height));
        if (oficio.Remetente.Equals("Vera"))
        {
            textFormatter.DrawString("Vera Cristina Carvalho Oliveira", fonteDescricao, corFonte, new XRect(0, 635, page.Width, page.Height));
            textFormatter.DrawString("Diretora - Port. 0314/2023", fonteDetalhes, corFonte, new XRect(0, 650, page.Width, page.Height));
        }
        else
        {
            textFormatter.DrawString("Alex de Oliveira Souza", fonteDescricao, corFonte, new XRect(0, 635, page.Width, page.Height));
            textFormatter.DrawString("Secretário - Port. 7083/2019", fonteDetalhes, corFonte, new XRect(0, 650, page.Width, page.Height));
        }

        // Destinatário
        textFormatter.Alignment = XParagraphAlignment.Left;
        textFormatter.DrawString("Illmº Senhor(a),", fonteDescricao, corFonte, new XRect(0, 670, page.Width, page.Height));
        textFormatter.DrawString(oficio.Destinatario, fonteDescricaoBold, corFonte, new XRect(0, 682, page.Width, page.Height));
        textFormatter.DrawString(oficio.CargoDoDestinatario, fonteDescricao, corFonte, new XRect(0, 694, page.Width, page.Height));
        textFormatter.DrawString(oficio.Cidade, fonteDescricao, corFonte, new XRect(0, 706, page.Width, page.Height));

        // Rodapé
        textFormatter.Alignment = XParagraphAlignment.Center;
        textFormatter.DrawString($"Ofício emitido em {oficio.DataEmissao:dd/MM/yyyy}", fonteDetalhes, corFonte, new XRect(0, 780, page.Width, page.Height));
        textFormatter.Alignment = XParagraphAlignment.Justify;
        textFormatter.DrawString(new string('_', 89), fonteDescricao, corFonte, new XRect(0, 750, page.Width, page.Height));
        textFormatter.DrawString("Este ofício foi gerado através do SGI da Escola Estadual João de Mattos Carvalho ", fonteRodape, corFonte, new XRect(0, 810, page.Width, page.Height));
        textFormatter.Alignment = XParagraphAlignment.Right;
        textFormatter.DrawString("Contato: eejmc.seed@seduc.se.gov.br ", fonteRodape, corFonte, new XRect(0, 810, page.Width, page.Height));

        using var stream = new MemoryStream();
        doc.Save(stream, false);
        return stream.ToArray();
    }

    // GET api/oficios/funcao/historico
    [HttpGet("funcao/historico")]
    public async Task<ActionResult<IEnumerable<OficioEmitidoDto>>> HistoricoFuncao()
    {
        var itens = await _context.OficiosFuncao.AsNoTracking()
            .OrderByDescending(o => o.DataEmissao)
            .Select(o => new OficioEmitidoDto(o.Id, o.NumeroOficio, o.Assunto, o.Nome, o.DataEmissao))
            .ToListAsync();

        return Ok(itens);
    }

    // POST api/oficios/funcao
    // Consolida os 3 tipos originais (Professor / Servidor / Apoio Escolar) num só endpoint,
    // diferenciados pelo campo Tipo.
    [HttpPost("funcao")]
    public async Task<IActionResult> GerarOficioFuncao(GerarOficioFuncaoRequest request)
    {
        if (request.Tipo is not ("Professor" or "Servidor" or "ApoioEscolar"))
            return BadRequest(new { mensagem = "Tipo inválido. Use Professor, Servidor ou ApoioEscolar." });

        var oficio = new OficioFuncao
        {
            Tipo = request.Tipo,
            NumeroOficio = request.NumeroOficio,
            Assunto = request.Assunto,
            Destinatario = request.Destinatario,
            CargoDestinatario = request.CargoDestinatario,
            CidadeDestinatario = request.CidadeDestinatario,
            SaudacaoGenero = request.SaudacaoGenero,
            Nome = request.Nome,
            CPF = request.CPF,
            Vinculo = request.Vinculo,
            DataAssumiuFuncao = DateTime.SpecifyKind(request.DataAssumiuFuncao, DateTimeKind.Utc),
            CargaHoraria = request.CargaHoraria,
            Cargo = request.Cargo,
            Disciplina = request.Disciplina,
            FonteRecursos = request.FonteRecursos,
            DataEmissao = DateTime.UtcNow,
            EmitidoPor = User.FindFirst("nomeCompleto")?.Value ?? User.Identity?.Name
        };

        var pdfBytes = GerarPdfFuncao(oficio);

        _context.OficiosFuncao.Add(oficio);
        await _context.SaveChangesAsync();

        var nomeArquivo = $"Oficio {oficio.Nome}.pdf";
        return File(pdfBytes, "application/pdf", nomeArquivo);
    }

    // Réplica/consolida SGI-JMC/Controllers/OficioAssumiuFuncao*.cs (gerarOficio)
    private byte[] GerarPdfFuncao(OficioFuncao oficio)
    {
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
        var fonteDescricaoBold = new XFont("Calibri", 14, XFontStyle.Bold);
        var fonteDetalhes = new XFont("Calibri", 10);
        var fonteRodape = new XFont("Calibri", 7);

        var imagensDir = Path.Combine(_env.ContentRootPath, "wwwroot", "Imagens");
        var imgBrasao = XImage.FromFile(Path.Combine(imagensDir, "BrasaoEstado.png"));
        var imgEscudo = XImage.FromFile(Path.Combine(imagensDir, "Escudo.png"));

        // Usuário (rodapé superior)
        textFormatter.Alignment = XParagraphAlignment.Right;
        textFormatter.DrawString("SGI-Sistema de Gerenciamento Interno - EEJMC ", fonteRodape, corFonte, new XRect(0, 30, page.Width, page.Height));
        textFormatter.DrawString($"Usuário: {oficio.EmitidoPor}", fonteRodape, corFonte, new XRect(0, 40, page.Width, page.Height));

        // Cabeçalho
        textFormatter.Alignment = XParagraphAlignment.Left;
        graphics.DrawImage(imgBrasao, 0, 30, 50, 75);
        graphics.DrawImage(imgEscudo, 75, 280, 450, 450);
        textFormatter.DrawString("GOVERNO DO ESTADO DE SERGIPE", fonteDescricao, corFonte, new XRect(55, 30, page.Width, page.Height));
        textFormatter.DrawString("SECRETARIA DE ESTADO DA EDUCAÇÃO E DA CULTURA", fonteDescricao, corFonte, new XRect(55, 45, page.Width, page.Height));
        textFormatter.DrawString("ESCOLA ESTADUAL JOÃO DE MATTOS CARVALHO", fonteDescricao, corFonte, new XRect(55, 60, page.Width, page.Height));
        textFormatter.DrawString("CNPJ: 01.902.194/0001-83", fonteDescricao, corFonte, new XRect(55, 75, page.Width, page.Height));
        textFormatter.DrawString("PRAÇA ABEL JACÓ DOS SANTOS, Nº 892, CENTRO, SIMÃO DIAS - SE", fonteDescricao, corFonte, new XRect(55, 90, page.Width, page.Height));
        textFormatter.DrawString(new string('_', 89), fonteDescricao, corFonte, new XRect(0, 100, page.Width, page.Height));

        // Início
        textFormatter.Alignment = XParagraphAlignment.Left;
        textFormatter.DrawString($"Ofício nº 00{oficio.NumeroOficio}/{oficio.DataEmissao.Year}", fonteDescricao, corFonte, new XRect(0, 150, page.Width, page.Height));
        textFormatter.DrawString($"Assunto: {oficio.Assunto}.", fonteDescricao, corFonte, new XRect(0, 165, page.Width, page.Height));
        textFormatter.DrawString($"Simão Dias - Se -  {oficio.DataEmissao:dd/MM/yyyy}", fonteDescricao, corFonte, new XRect(0, 200, page.Width, page.Height));

        // Saudação (generalizada por gênero, em vez de checar nomes fixos como no original)
        var ehSenhora = oficio.SaudacaoGenero.Equals("Senhora", StringComparison.OrdinalIgnoreCase);
        textFormatter.DrawString(
            ehSenhora ? "Senhora Diretora," : "Senhor Diretor,",
            fonteDescricao, corFonte, new XRect(0, 280, page.Width, page.Height));

        // Corpo do ofício — texto varia conforme o tipo de vínculo
        var ch = oficio.CargaHoraria > 50 ? "mensais" : "semanais";
        string corpo;

        if (oficio.Tipo == "Professor")
        {
            var frcTexto = oficio.FonteRecursos is int frc && frc > 0
                ? $", atuando no Ensino Fundamental FRC {frc} (FRC: Fonte de Recursos do FUNDEB)."
                : ".";
            corpo = $"Comunicamos a Vossa Senhoria que {oficio.Nome}, CPF {oficio.CPF}, vínculo {oficio.Vinculo}, " +
                    $"ocupante do Cargo de Professor de Educação Básica, assumiu suas funções em regência de classe " +
                    $"no dia {oficio.DataAssumiuFuncao:dd/MM/yyyy} com carga horária de {oficio.CargaHoraria} horas {ch}, " +
                    $"na disciplina, {oficio.Disciplina} conforme horário anexo{frcTexto}";
        }
        else if (oficio.Tipo == "Servidor")
        {
            corpo = $"Comunicamos a Vossa Senhoria que {oficio.Nome}, CPF {oficio.CPF}, vínculo {oficio.Vinculo}, " +
                    $"ocupante do Cargo de {oficio.Cargo}, assumiu suas funções no dia {oficio.DataAssumiuFuncao:dd/MM/yyyy} " +
                    $"com carga horária de {oficio.CargaHoraria} horas {ch}, conforme horário anexo.";
        }
        else // ApoioEscolar
        {
            corpo = $"Comunicamos a Vossa Senhoria que {oficio.Nome}, CPF {oficio.CPF}, vínculo {oficio.Vinculo}(a), " +
                    $"ocupante do Cargo de Apoio Escolar II, assumiu suas funções no dia {oficio.DataAssumiuFuncao:dd/MM/yyyy} " +
                    $"com carga horária de {oficio.CargaHoraria} horas mensais, conforme horário anexo.";
        }

        textFormatter.Alignment = XParagraphAlignment.Justify;
        textFormatter.DrawString(corpo, fonteDescricao, corFonte, new XRect(0, 330, page.Width, page.Height));

        // Assinatura (sempre a Diretora, igual ao sistema original para este tipo de ofício)
        textFormatter.Alignment = XParagraphAlignment.Center;
        textFormatter.DrawString(new string('_', 60), fonteDetalhes, corFonte, new XRect(0, 500, page.Width, page.Height));
        textFormatter.DrawString("Vera Cristina Carvalho Oliveira", fonteDescricao, corFonte, new XRect(0, 510, page.Width, page.Height));
        textFormatter.DrawString("Diretora - Port. 0314/2023", fonteDetalhes, corFonte, new XRect(0, 525, page.Width, page.Height));

        // Destinatário
        textFormatter.Alignment = XParagraphAlignment.Left;
        textFormatter.DrawString(ehSenhora ? "Illma Senhora," : "Illmo Senhor,", fonteDescricao, corFonte, new XRect(0, 670, page.Width, page.Height));
        textFormatter.DrawString(oficio.Destinatario, fonteDescricaoBold, corFonte, new XRect(0, 682, page.Width, page.Height));
        textFormatter.DrawString(oficio.CargoDestinatario, fonteDescricao, corFonte, new XRect(0, 694, page.Width, page.Height));
        textFormatter.DrawString(oficio.CidadeDestinatario, fonteDescricao, corFonte, new XRect(0, 706, page.Width, page.Height));

        // Rodapé
        textFormatter.Alignment = XParagraphAlignment.Center;
        textFormatter.DrawString($"Ofício emitido em {oficio.DataEmissao:dd/MM/yyyy}", fonteDetalhes, corFonte, new XRect(0, 780, page.Width, page.Height));
        textFormatter.Alignment = XParagraphAlignment.Justify;
        textFormatter.DrawString(new string('_', 89), fonteDescricao, corFonte, new XRect(0, 750, page.Width, page.Height));
        textFormatter.DrawString("Este ofício foi gerado através do SGI da Escola Estadual João de Mattos Carvalho ", fonteRodape, corFonte, new XRect(0, 810, page.Width, page.Height));
        textFormatter.Alignment = XParagraphAlignment.Right;
        textFormatter.DrawString("Contato: eejmc.seed@seduc.se.gov.br ", fonteRodape, corFonte, new XRect(0, 810, page.Width, page.Height));

        using var stream = new MemoryStream();
        doc.Save(stream, false);
        return stream.ToArray();
    }
}