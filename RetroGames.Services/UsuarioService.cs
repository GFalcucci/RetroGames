
using Microsoft.AspNetCore.Identity;
using RetroGames.Model.Constantes;
using RetroGames.Model.Model;
using RetroGames.Repository;
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

    public async Task<Usuario> CadastrarClienteAsync(
        string nome,
        string email,
        string senha)
    {
        var emailNormalizado = email.Trim();

        if (await _usuarioRepository.BuscarPorEmailAsync(emailNormalizado) != null)
        {
            throw new BusinessRuleException(BusinessErrors.EmailJaCadastrado);
        }

        var cliente = new Usuario
        {
            Nome = nome.Trim(),
            Email = emailNormalizado,
            Ativo = true,
            DataCadastro = DateTime.UtcNow,
            IdPerfil = PerfilIds.Cliente
        };

        cliente.SenhaHash = _passwordHasher.HashPassword(cliente, senha);

        try
        {
            cliente.Id = await _usuarioRepository.InserirAsync(cliente);
        }
        catch (RegistroDuplicadoException)
        {
            // Duas requisições simultâneas com o mesmo e-mail.
            throw new BusinessRuleException(BusinessErrors.EmailJaCadastrado);
        }

        return cliente;
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
