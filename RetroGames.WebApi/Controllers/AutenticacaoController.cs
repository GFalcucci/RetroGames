
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetroGames.Services.Interfaces;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AutenticacaoController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;

    public AutenticacaoController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<ResponseApiModel<object?>>> Login(
        LoginRequestApiModel modelo)
    {
        var usuario = await _usuarioService.AutenticarAsync(
            modelo.Email,
            modelo.Senha);

        if (usuario == null)
        {
            var erro = new ResponseApiModel<object?>
            {
                ResponseCode = ResponseCodes.CREDENCIAIS_INVALIDAS.Code,
                Data = null,
                ErrorMessage = ResponseCodes.CREDENCIAIS_INVALIDAS.DefaultMessage
            };

            return Unauthorized(erro);
        }

        var identidade = new ClaimsIdentity(
            new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nome),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim("IdPerfil", usuario.IdPerfil.ToString()),
                new Claim(
                    ClaimTypes.Role,
                    usuario.IdPerfil == 1 ? "Administrador" : "Cliente")
            },
            "Cookies");

        var principal = new ClaimsPrincipal(identidade);

        await HttpContext.SignInAsync(principal);

        var resposta = new ResponseApiModel<UsuarioAutenticadoResponseApiModel>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = new UsuarioAutenticadoResponseApiModel
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                IdPerfil = usuario.IdPerfil
            },
            ErrorMessage = null
        };

        return Ok(resposta);
    }

    [HttpPost("logout")]
    public async Task<ActionResult<ResponseApiModel<object?>>> Logout()
    {
        await HttpContext.SignOutAsync();

        var resposta = new ResponseApiModel<object?>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = null,
            ErrorMessage = null
        };

        return Ok(resposta);
    }

    [HttpGet("eu")]
    public ActionResult<ResponseApiModel<UsuarioAutenticadoResponseApiModel>> Eu()
    {
        var resposta = new ResponseApiModel<UsuarioAutenticadoResponseApiModel>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = new UsuarioAutenticadoResponseApiModel
            {
                Id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value),
                Nome = User.FindFirst(ClaimTypes.Name)!.Value,
                Email = User.FindFirst(ClaimTypes.Email)!.Value,
                IdPerfil = int.Parse(User.FindFirst("IdPerfil")!.Value)
            },
            ErrorMessage = null
        };

        return Ok(resposta);
    }
}
