using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SGI_JMC.Api.Authorization;
using SGI_JMC.Api.Models;

namespace SGI_JMC.Api.Data;

// Migrado de SGI-JMC/Models/Inicializador.cs, adaptado para rodar no startup da API
public static class DbSeeder
{
    // "usuario" mantido por compatibilidade com contas criadas antes do controle de acesso
    // por perfil — equivale, na prática, ao perfil "secretario".
    private static readonly string[] Roles =
    {
        Perfis.Administrador, Perfis.Diretor, Perfis.Secretario, Perfis.OficialAdministrativo, "usuario"
    };

    public static async Task SeedAsync(IServiceProvider services)
    {
        var context = services.GetRequiredService<AppDbContext>();
        await context.Database.MigrateAsync();

        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        foreach (var role in Roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        await SeedPermissoesPadraoAsync(context);

        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var config = services.GetRequiredService<IConfiguration>();

        var adminEmail = config["AdminSeed:Email"] ?? "admin@sgi-jmc.local";
        var adminSenha = config["AdminSeed:Senha"] ?? "admin123";

        if (await userManager.FindByEmailAsync(adminEmail) is null)
        {
            var admin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                NomeCompleto = "Administrador",
                EmailConfirmed = true
            };
            var resultado = await userManager.CreateAsync(admin, adminSenha);
            if (resultado.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, Perfis.Administrador);
            }
            else
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                foreach (var erro in resultado.Errors)
                    logger.LogError("Falha ao criar usuário admin no seed: {Codigo} - {Descricao}", erro.Code, erro.Description);
            }
        }
    }

    // Define quais módulos cada perfil enxerga por padrão na primeira execução.
    // Tudo isso é editável depois em Administração → Permissões por perfil.
    private static async Task SeedPermissoesPadraoAsync(AppDbContext context)
    {
        if (await context.PerfilPermissoes.AnyAsync())
            return; // já foi semeado antes — não sobrescreve o que foi configurado manualmente

        var padroes = new Dictionary<string, string[]>
        {
            // Diretor: acesso a tudo por padrão (pode restringir depois se quiser)
            [Perfis.Diretor] = Modulos.Todos.Select(m => m.Chave).ToArray(),

            // Secretário: rotina do dia a dia da secretaria
            [Perfis.Secretario] = new[]
            {
                Modulos.Alunos, Modulos.Declaracoes, Modulos.Oficios,
                Modulos.Horarios, Modulos.Ocorrencias,
            },

            // Oficial administrativo: tarefas mais restritas
            [Perfis.OficialAdministrativo] = new[]
            {
                Modulos.Alunos, Modulos.Declaracoes, Modulos.Oficios,
            },
        };

        var permissoes = new List<PerfilPermissao>();
        foreach (var (perfil, modulosLiberados) in padroes)
        {
            foreach (var (chave, _) in Modulos.Todos)
            {
                permissoes.Add(new PerfilPermissao
                {
                    Perfil = perfil,
                    ModuloChave = chave,
                    Permitido = modulosLiberados.Contains(chave)
                });
            }
        }

        context.PerfilPermissoes.AddRange(permissoes);
        await context.SaveChangesAsync();
    }
}
