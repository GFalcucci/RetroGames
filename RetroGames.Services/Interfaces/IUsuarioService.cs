
using RetroGames.Model.Model;

namespace RetroGames.Services.Interfaces;

public interface IUsuarioService
{
    Task<Usuario?> AutenticarAsync(string email, string senha);

    // Cria um usuário ativo com perfil Cliente e senha em hash.
    Task<Usuario> CadastrarClienteAsync(string nome, string email, string senha);

    Task GarantirAdministradorInicialAsync(string email, string senha);
}
