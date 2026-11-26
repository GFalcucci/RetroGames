
using RetroGames.Model.Model;

namespace RetroGames.Services.Interfaces;

public interface IUsuarioService
{
    Task<Usuario?> AutenticarAsync(string email, string senha);

    Task GarantirAdministradorInicialAsync(string email, string senha);
}
