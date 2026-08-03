using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SGI_JMC.Api.Authorization;
using SGI_JMC.Api.DTOs;
using SGI_JMC.Api.Models;

namespace SGI_JMC.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "administrador")]
public class UsuariosController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UsuariosController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    private static DateTime? ComoUtc(DateTime? data) =>
        data is null ? null : DateTime.SpecifyKind(data.Value, DateTimeKind.Utc);

    // GET api/usuarios
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UsuarioAdminDto>>> Listar()
    {
        var usuarios = _userManager.Users.ToList();
        var resultado = new List<UsuarioAdminDto>();

        foreach (var usuario in usuarios)
        {
            var roles = await _userManager.GetRolesAsync(usuario);
            resultado.Add(new UsuarioAdminDto(usuario.Id, usuario.NomeCompleto, usuario.Email!, roles.ToList(), usuario.EmailConfirmed, usuario.Ativo, usuario.CPF, usuario.DataNascimento));
        }

        return Ok(resultado.OrderBy(u => u.NomeCompleto));
    }

    // PUT api/usuarios/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Editar(string id, AtualizarUsuarioRequest request)
    {
        var usuario = await _userManager.FindByIdAsync(id);
        if (usuario is null) return NotFound();

        usuario.NomeCompleto = request.NomeCompleto;
        usuario.CPF = request.CPF;
        usuario.DataNascimento = ComoUtc(request.DataNascimento);

        if (!string.Equals(usuario.Email, request.Email, StringComparison.OrdinalIgnoreCase))
        {
            var emailExistente = await _userManager.FindByEmailAsync(request.Email);
            if (emailExistente is not null && emailExistente.Id != usuario.Id)
                return Conflict(new { mensagem = "Já existe um usuário cadastrado com este e-mail." });

            var resultadoEmail = await _userManager.SetEmailAsync(usuario, request.Email);
            if (!resultadoEmail.Succeeded)
                return BadRequest(new { erros = resultadoEmail.Errors.Select(e => e.Description) });

            var resultadoUserName = await _userManager.SetUserNameAsync(usuario, request.Email);
            if (!resultadoUserName.Succeeded)
                return BadRequest(new { erros = resultadoUserName.Errors.Select(e => e.Description) });
        }
        else
        {
            await _userManager.UpdateAsync(usuario);
        }

        return NoContent();
    }

    // PUT api/usuarios/{id}/status
    [HttpPut("{id}/status")]
    public async Task<IActionResult> AtualizarStatus(string id, AtualizarStatusUsuarioRequest request)
    {
        var usuario = await _userManager.FindByIdAsync(id);
        if (usuario is null) return NotFound();

        if (usuario.Id == User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value)
            return BadRequest(new { mensagem = "Você não pode inativar sua própria conta." });

        usuario.Ativo = request.Ativo;
        await _userManager.UpdateAsync(usuario);

        return NoContent();
    }

    // PUT api/usuarios/{id}/perfil
    [HttpPut("{id}/perfil")]
    public async Task<IActionResult> AtualizarPerfil(string id, AtualizarPerfilUsuarioRequest request)
    {
        var perfisValidos = Perfis.Todos.Select(p => p.Chave);
        if (!perfisValidos.Contains(request.NovoPerfil))
            return BadRequest(new { mensagem = "Perfil inválido." });

        var usuario = await _userManager.FindByIdAsync(id);
        if (usuario is null) return NotFound();

        var rolesAtuais = await _userManager.GetRolesAsync(usuario);
        await _userManager.RemoveFromRolesAsync(usuario, rolesAtuais);
        await _userManager.AddToRoleAsync(usuario, request.NovoPerfil);

        return NoContent();
    }

    // DELETE api/usuarios/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Remover(string id)
    {
        var usuario = await _userManager.FindByIdAsync(id);
        if (usuario is null) return NotFound();

        if (usuario.Id == User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value)
            return BadRequest(new { mensagem = "Você não pode excluir sua própria conta." });

        await _userManager.DeleteAsync(usuario);
        return NoContent();
    }
}
