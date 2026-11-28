
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Repository.Interfaces;
using RetroGames.Repository.Repositories;
using RetroGames.Services;
using RetroGames.Services.Interfaces;

namespace RetroGames.WebApi.Configuration;

public static class InjecaoDependenciaConfig
{
    public static IServiceCollection AddRepositorios(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddSingleton(new RepositoryConfig
        {
            ConnectionString = connectionString
        });

        services.AddScoped<IPlataformaRepository, PlataformaRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();

        services.AddScoped<IRepositorySet, RepositorySet>();

        return services;
    }

    public static IServiceCollection AddServicosDeNegocio(
        this IServiceCollection services)
    {
        services.AddScoped<IPlataformaService, PlataformaService>();
        services.AddScoped<IGeneroService, GeneroService>();
        services.AddScoped<ICondicaoJogoService, CondicaoJogoService>();
        services.AddScoped<IJogoService, JogoService>();
        services.AddScoped<IUsuarioService, UsuarioService>();
        services.AddScoped<IPedidoService, PedidoService>();

        services.AddScoped<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();

        return services;
    }
}
