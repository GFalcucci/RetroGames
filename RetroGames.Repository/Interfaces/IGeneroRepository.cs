using RetroGames.Model.Model;

namespace RetroGames.Repository.Interfaces;

public interface IGeneroRepository
{
    // Lista todos os gêneros.
    Task<List<Genero>> ListarAsync();
}