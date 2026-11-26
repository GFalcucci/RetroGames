
using Microsoft.AspNetCore.Identity;
using RetroGames.Model.Model;
using RetroGames.Repository.Interfaces;
using RetroGames.Services.Interfaces;

namespace RetroGames.Services;

public class UsuarioService : IUsuarioService
{
    private const int PERFIL_ADMINISTRADOR = 1;

    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPasswordHasher<Usuario> _passwordHasher;

    public UsuarioService(
        IUsuarioRepository usuarioRepository,
        IPasswordHasher<Usuario> passwordHasher)
    {
        _usuarioRepository = usuarioRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<Usuario?> AutenticarAsync(string email, string senha)
    {
        var usuario = await _usuarioRepository.BuscarPorEmailAsync(email);

        if (usuario == null || !usuario.Ativo)
        {
            return null;
        }

        var resultado = _passwordHasher.VerifyHashedPassword(
            usuario,
            usuario.SenhaHash,
            senha);

        if (resultado == PasswordVerificationResult.Failed)
        {
            return null;
        }

        return usuario;
    }

    public async Task GarantirAdministradorInicialAsync(
        string email,
        string senha)
    {
        if (string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(senha))
        {
            throw new InvalidOperationException(
                "O e-mail e a senha do administrador inicial devem ser configurados no .env.");
        }

        var administradorExiste =
            await _usuarioRepository.ExisteComPerfilAsync(
                PERFIL_ADMINISTRADOR);

        if (administradorExiste)
        {
            return;
        }

        var usuarioExistente =
            await _usuarioRepository.BuscarPorEmailAsync(email);

        if (usuarioExistente != null)
        {
            throw new InvalidOperationException(
                "O e-mail configurado para o administrador inicial já pertence a outro usuário.");
        }

        var administrador = new Usuario
        {
            Nome = "Administrador",
            Email = email,
            Ativo = true,
            DataCadastro = DateTime.Now,
            IdPerfil = PERFIL_ADMINISTRADOR
        };

        administrador.SenhaHash = _passwordHasher.HashPassword(
            administrador,
            senha);

        await _usuarioRepository.InserirAsync(administrador);
    }
}
