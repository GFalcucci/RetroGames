using RetroGames.Model.Model;

namespace RetroGames.Repository.Interfaces;

public interface ICondicaoJogoRepository
{
    // Lista todas as condições de jogo.
    Task<List<CondicaoJogo>> ListarAsync();
}