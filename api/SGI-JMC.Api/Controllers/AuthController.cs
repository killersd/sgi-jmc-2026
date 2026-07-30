using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<IdentityRole> roleManager,
        ITokenService tokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _tokenService = tokenService;
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

        var roles = await _userManager.GetRolesAsync(usuario);
        var (token, expiraEm) = _tokenService.GerarToken(usuario, roles);

        return Ok(new AuthResponse(token, expiraEm, usuario.Id, usuario.NomeCompleto, usuario.Email!, roles));
    }

    [HttpPost("registrar")]
    [Authorize(Roles = "administrador")]
    public async Task<ActionResult<AuthResponse>> Registrar(RegisterRequest request)
    {
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
            EmailConfirmed = true
        };

        var resultado = await _userManager.CreateAsync(usuario, request.Senha);
        if (!resultado.Succeeded)
            return BadRequest(new { erros = resultado.Errors.Select(e => e.Description) });

        await _userManager.AddToRoleAsync(usuario, "usuario");

        var roles = await _userManager.GetRolesAsync(usuario);
        var (token, expiraEm) = _tokenService.GerarToken(usuario, roles);

        return Ok(new AuthResponse(token, expiraEm, usuario.Id, usuario.NomeCompleto, usuario.Email!, roles));
    }

    [HttpGet("perfil")]
    [Authorize]
    public async Task<ActionResult<AuthResponse>> Perfil()
    {
        var usuario = await _userManager.GetUserAsync(User);
        if (usuario is null) return Unauthorized();

        var roles = await _userManager.GetRolesAsync(usuario);
        return Ok(new { usuario.Id, usuario.NomeCompleto, usuario.Email, roles });
    }
}
