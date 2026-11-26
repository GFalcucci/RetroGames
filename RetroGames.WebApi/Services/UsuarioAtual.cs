using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using RetroGames.Services.Interfaces;

namespace RetroGames.WebApi.Services;

public class UsuarioAtual : IUsuarioAtual
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UsuarioAtual(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? Usuario =>
        _httpContextAccessor.HttpContext?.User;

    public int? Id =>
        int.TryParse(
            Usuario?.FindFirst(ClaimTypes.NameIdentifier)?.Value,
            out var id)
                ? id
                : null;

    public int? IdPerfil =>
        int.TryParse(
            Usuario?.FindFirst("IdPerfil")?.Value,
            out var idPerfil)
                ? idPerfil
                : null;

    public bool EhAdministrador =>
        Usuario?.IsInRole("Administrador") == true;
}