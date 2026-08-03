using System.Security.Claims;
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
[Authorize(Roles = "administrador,diretor")]
public class PerfisController : ControllerBase
{
    private readonly AppDbContext _context;

    public PerfisController(AppDbContext context)
    {
        _context = context;
    }

    // Perfis que ESTE usuário (administrador ou diretor) tem permissão de configurar.
    private string[] PerfisEditaveisPeloUsuarioAtual()
    {
        var perfisDoUsuario = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        return perfisDoUsuario.Contains(Perfis.Administrador)
            ? Perfis.ConfiguraveisPeloAdministrador
            : Perfis.ConfiguraveisPeloDiretor;
    }

    // GET api/perfis/matriz
    [HttpGet("matriz")]
    public async Task<ActionResult<MatrizPermissoesDto>> ObterMatriz()
    {
        var perfisEditaveis = PerfisEditaveisPeloUsuarioAtual();

        var permissoes = await _context.PerfilPermissoes.AsNoTracking()
            .Where(p => perfisEditaveis.Contains(p.Perfil))
            .Select(p => new PermissaoDto(p.Perfil, p.ModuloChave, p.Permitido))
            .ToListAsync();

        var modulos = Modulos.Todos.Select(m => new ModuloDto(m.Chave, m.Nome)).ToList();
        var perfisDto = Perfis.Todos
            .Where(p => perfisEditaveis.Contains(p.Chave))
            .Select(p => new PerfilDto(p.Chave, p.Nome))
            .ToList();

        return Ok(new MatrizPermissoesDto(modulos, perfisDto, permissoes));
    }

    // PUT api/perfis/matriz
    [HttpPut("matriz")]
    public async Task<IActionResult> AtualizarMatriz(AtualizarPermissoesRequest request)
    {
        var perfisEditaveis = PerfisEditaveisPeloUsuarioAtual();

        var moduloChaves = Modulos.Todos.Select(m => m.Chave).ToHashSet();

        foreach (var permissao in request.Permissoes)
        {
            if (!perfisEditaveis.Contains(permissao.Perfil))
                return Forbid();
            if (!moduloChaves.Contains(permissao.ModuloChave))
                return BadRequest(new { mensagem = $"Módulo inválido: {permissao.ModuloChave}" });
        }

        foreach (var permissao in request.Permissoes)
        {
            var existente = await _context.PerfilPermissoes
                .FirstOrDefaultAsync(p => p.Perfil == permissao.Perfil && p.ModuloChave == permissao.ModuloChave);

            if (existente is null)
            {
                _context.PerfilPermissoes.Add(new PerfilPermissao
                {
                    Perfil = permissao.Perfil,
                    ModuloChave = permissao.ModuloChave,
                    Permitido = permissao.Permitido
                });
            }
            else
            {
                existente.Permitido = permissao.Permitido;
            }
        }

        await _context.SaveChangesAsync();
        return NoContent();
    }
}
