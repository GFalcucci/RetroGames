
using Microsoft.AspNetCore.Authorization;

namespace RetroGames.WebApi.Configuration;

public static class AutorizacaoConfig
{
    public static IServiceCollection AdicionarAutorizacao(
        this IServiceCollection servicos)
    {
        servicos.AddAuthorization(opcoes =>
        {
            // Política que exige apenas um usuário autenticado.
            opcoes.AddPolicy(Politicas.UsuarioAutenticado, politica =>
            {
                politica.RequireAuthenticatedUser();
            });

            // Política que exige um usuário com perfil Administrador.
            opcoes.AddPolicy(Politicas.Administrador, politica =>
            {
                politica.RequireAuthenticatedUser();
                politica.RequireRole("Administrador");
            });

            // Exige autenticação em todas as rotas por padrão.
            opcoes.FallbackPolicy =
                opcoes.GetPolicy(Politicas.UsuarioAutenticado);
        });

        return servicos;
    }
}
