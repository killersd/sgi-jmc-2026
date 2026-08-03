using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SGI_JMC.Api.Data;
using SGI_JMC.Api.DTOs;
using SGI_JMC.Api.Models;

namespace SGI_JMC.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "administrador")]
public class ConfiguracoesController : ControllerBase
{
    private readonly AppDbContext _context;

    public ConfiguracoesController(AppDbContext context)
    {
        _context = context;
    }

    private async Task<ConfiguracaoNotificacoes> ObterOuCriarAsync()
    {
        var configuracao = await _context.ConfiguracoesNotificacoes.FirstOrDefaultAsync();
        if (configuracao is not null)
            return configuracao;

        configuracao = new ConfiguracaoNotificacoes();
        _context.ConfiguracoesNotificacoes.Add(configuracao);
        await _context.SaveChangesAsync();
        return configuracao;
    }

    // GET api/configuracoes/notificacoes
    [HttpGet("notificacoes")]
    public async Task<ActionResult<ConfiguracaoNotificacoesDto>> ObterNotificacoes()
    {
        var configuracao = await ObterOuCriarAsync();
        return Ok(new ConfiguracaoNotificacoesDto(configuracao.EmailNotificacaoTransferencia));
    }

    // PUT api/configuracoes/notificacoes
    [HttpPut("notificacoes")]
    public async Task<IActionResult> AtualizarNotificacoes(ConfiguracaoNotificacoesDto request)
    {
        var configuracao = await ObterOuCriarAsync();
        configuracao.EmailNotificacaoTransferencia = string.IsNullOrWhiteSpace(request.EmailNotificacaoTransferencia)
            ? null
            : request.EmailNotificacaoTransferencia.Trim();

        await _context.SaveChangesAsync();
        return NoContent();
    }
}
