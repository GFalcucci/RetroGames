
using RetroGames.Model.Model;

namespace RetroGames.Repository.Interfaces;

public interface IUsuarioRepository
{
    Task<Usuario?> BuscarPorIdAsync(int id);
    Task<Usuario?> BuscarPorEmailAsync(string email);
    Task<bool> ExisteComPerfilAsync(int idPerfil);
    Task<int> InserirAsync(Usuario usuario);
}
