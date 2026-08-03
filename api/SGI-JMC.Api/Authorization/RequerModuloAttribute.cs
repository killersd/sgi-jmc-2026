using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using SGI_JMC.Api.Data;

namespace SGI_JMC.Api.Authorization;

// Aplique em um controller ou action: [RequerModulo(Modulos.Alunos)]
// O perfil "administrador" sempre passa. Os demais perfis só passam se tiverem
// uma permissão explícita (Permitido = true) para aquele módulo na tabela PerfilPermissoes.
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RequerModuloAttribute : Attribute, IAsyncAuthorizationFilter
{
    private readonly string _modulo;

    public RequerModuloAttribute(string modulo)
    {
        _modulo = modulo;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;
        if (user.Identity is not { IsAuthenticated: true })
        {
            context.Result = new Microsoft.AspNetCore.Mvc.UnauthorizedResult();
            return;
        }

        var perfisDoUsuario = user.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        if (perfisDoUsuario.Contains(Perfis.Administrador))
            return; // acesso total

        var db = context.HttpContext.RequestServices.GetRequiredService<AppDbContext>();
        var permitido = await db.PerfilPermissoes.AsNoTracking()
            .AnyAsync(p => perfisDoUsuario.Contains(p.Perfil) && p.ModuloChave == _modulo && p.Permitido);

        if (!permitido)
        {
            context.Result = new Microsoft.AspNetCore.Mvc.ObjectResult(new { mensagem = "Seu perfil não tem acesso a este módulo." })
            {
                StatusCode = 403
            };
        }
    }
}
