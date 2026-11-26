
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Configuration;

public static class AutenticacaoConfig
{
    public static IServiceCollection AdicionarAutenticacao(
        this IServiceCollection servicos)
    {
        servicos
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(opcoes =>
            {
                opcoes.Cookie.HttpOnly = true;
                opcoes.Cookie.SameSite = SameSiteMode.Strict;

                opcoes.ExpireTimeSpan = TimeSpan.FromHours(2);
                opcoes.SlidingExpiration = true;

                opcoes.Events.OnRedirectToLogin = async contexto =>
                {
                    contexto.Response.StatusCode =
                        StatusCodes.Status401Unauthorized;

                    var resposta = new ResponseApiModel<object?>
                    {
                        ResponseCode = ResponseCodes.NAO_AUTENTICADO.Code,
                        Data = null,
                        ErrorMessage = ResponseCodes.NAO_AUTENTICADO.DefaultMessage
                    };

                    await contexto.Response.WriteAsJsonAsync(resposta);
                };

                opcoes.Events.OnRedirectToAccessDenied = async contexto =>
                {
                    contexto.Response.StatusCode =
                        StatusCodes.Status403Forbidden;

                    var resposta = new ResponseApiModel<object?>
                    {
                        ResponseCode = ResponseCodes.ACESSO_NEGADO.Code,
                        Data = null,
                        ErrorMessage = ResponseCodes.ACESSO_NEGADO.DefaultMessage
                    };

                    await contexto.Response.WriteAsJsonAsync(resposta);
                };
            });

        return servicos;
    }
}
