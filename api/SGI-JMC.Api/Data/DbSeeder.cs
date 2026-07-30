using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SGI_JMC.Api.Models;

namespace SGI_JMC.Api.Data;

// Migrado de SGI-JMC/Models/Inicializador.cs, adaptado para rodar no startup da API
public static class DbSeeder
{
    private static readonly string[] Roles = { "administrador", "usuario" };

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
                await userManager.AddToRoleAsync(admin, "administrador");
            }
            else
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                foreach (var erro in resultado.Errors)
                    logger.LogError("Falha ao criar usuário admin no seed: {Codigo} - {Descricao}", erro.Code, erro.Description);
            }
        }
    }
}
