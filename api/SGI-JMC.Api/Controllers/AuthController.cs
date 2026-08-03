using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SGI_JMC.Api.Authorization;
using SGI_JMC.Api.Data;
using SGI_JMC.Api.DTOs;
using SGI_JMC.Api.Models;
using SGI_JMC.Api.Services;

namespace SGI_JMC.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ITokenService _tokenService;
    private readonly AppDbContext _context;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<IdentityRole> roleManager,
        ITokenService tokenService,
        AppDbContext context,
        IEmailService emailService,
        IConfiguration configuration,
        ILogger<AuthController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _tokenService = tokenService;
        _context = context;
        _emailService = emailService;
        _configuration = configuration;
        _logger = logger;
    }

    // Calcula quais módulos o conjunto de perfis do usuário libera.
    // "administrador" sempre tem todos; os demais dependem da tabela PerfilPermissoes.
    private async Task<List<string>> CalcularModulosPermitidosAsync(IList<string> roles)
    {
        if (roles.Contains(Perfis.Administrador))
            return Modulos.Todos.Select(m => m.Chave).ToList();

        return await _context.PerfilPermissoes.AsNoTracking()
            .Where(p => roles.Contains(p.Perfil) && p.Permitido)
            .Select(p => p.ModuloChave)
            .Distinct()
            .ToListAsync();
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var usuario = await _userManager.FindByEmailAsync(request.Email);
        if (usuario is null)
            return Unauthorized(new { mensagem = "E-mail ou senha inválidos." });

        var resultado = await _signInManager.CheckPasswordSignInAsync(usuario, request.Senha, lockoutOnFailure: true);
        if (!resultado.Succeeded)
        {
            if (resultado.IsLockedOut)
                return Unauthorized(new { mensagem = "Conta bloqueada temporariamente por tentativas inválidas." });
            return Unauthorized(new { mensagem = "E-mail ou senha inválidos." });
        }

        if (!usuario.EmailConfirmed)
            return StatusCode(403, new { codigo = "EMAIL_NAO_CONFIRMADO", mensagem = "Confirme seu e-mail antes de entrar. Verifique o link de ativação enviado à sua caixa de entrada." });

        if (!usuario.Ativo)
            return StatusCode(403, new { codigo = "CONTA_INATIVA", mensagem = "Sua conta foi desativada. Entre em contato com o administrador." });

        var roles = await _userManager.GetRolesAsync(usuario);
        var (token, expiraEm) = _tokenService.GerarToken(usuario, roles);
        var modulos = await CalcularModulosPermitidosAsync(roles);

        return Ok(new AuthResponse(token, expiraEm, usuario.Id, usuario.NomeCompleto, usuario.Email!, roles, modulos));
    }

    [HttpPost("registrar")]
    [Authorize(Roles = "administrador")]
    public async Task<ActionResult<UsuarioCriadoDto>> Registrar(RegisterRequest request)
    {
        var perfisValidos = Perfis.Todos.Select(p => p.Chave);
        if (!perfisValidos.Contains(request.Perfil))
            return BadRequest(new { mensagem = "Perfil inválido." });

        var existente = await _userManager.FindByEmailAsync(request.Email);
        if (existente is not null)
            return Conflict(new { mensagem = "Já existe um usuário cadastrado com este e-mail." });

        var usuario = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            NomeCompleto = request.NomeCompleto,
            CPF = request.CPF,
            DataNascimento = request.DataNascimento,
            EmailConfirmed = false
        };

        var resultado = await _userManager.CreateAsync(usuario);
        if (!resultado.Succeeded)
            return BadRequest(new { erros = resultado.Errors.Select(e => e.Description) });

        await _userManager.AddToRoleAsync(usuario, request.Perfil);

        var emailEnviado = await EnviarEmailDeAtivacaoAsync(usuario);

        return Ok(new UsuarioCriadoDto(usuario.Id, usuario.NomeCompleto, usuario.Email!, emailEnviado));
    }

    private async Task<bool> EnviarEmailDeAtivacaoAsync(ApplicationUser usuario)
    {
        try
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(usuario);
            var baseUrl = _configuration["Frontend:BaseUrl"] ?? "http://localhost:5173";
            var link = $"{baseUrl}/ativar-conta?userId={usuario.Id}&token={Uri.EscapeDataString(token)}";

            var corpoHtml =
                $"<p>Olá, {usuario.NomeCompleto}.</p>" +
                $"<p>Uma conta foi criada para você no SGI-JMC. Clique no link abaixo para definir sua senha e ativar seu acesso:</p>" +
                $"<p><a href=\"{link}\">{link}</a></p>";

            await _emailService.EnviarAsync(usuario.Email!, "Ative seu acesso ao SGI-JMC", corpoHtml);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao enviar e-mail de ativação para {Email}", usuario.Email);
            return false;
        }
    }

    [HttpPost("ativar-conta")]
    public async Task<IActionResult> AtivarConta(AtivarContaRequest request)
    {
        var usuario = await _userManager.FindByIdAsync(request.UserId);
        if (usuario is null)
            return BadRequest(new { mensagem = "Link de ativação inválido ou expirado." });

        if (usuario.EmailConfirmed)
            return BadRequest(new { mensagem = "Esta conta já foi ativada." });

        var confirmacao = await _userManager.ConfirmEmailAsync(usuario, request.Token);
        if (!confirmacao.Succeeded)
            return BadRequest(new { mensagem = "Link de ativação inválido ou expirado." });

        var definirSenha = await _userManager.AddPasswordAsync(usuario, request.NovaSenha);
        if (!definirSenha.Succeeded)
            return BadRequest(new { erros = definirSenha.Errors.Select(e => e.Description) });

        return NoContent();
    }

    [HttpGet("perfil")]
    [Authorize]
    public async Task<ActionResult<AuthResponse>> Perfil()
    {
        var usuario = await _userManager.GetUserAsync(User);
        if (usuario is null) return Unauthorized();

        var roles = await _userManager.GetRolesAsync(usuario);
        var modulos = await CalcularModulosPermitidosAsync(roles);

        return Ok(new AuthResponse("", DateTime.MinValue, usuario.Id, usuario.NomeCompleto, usuario.Email!, roles, modulos));
    }
}
