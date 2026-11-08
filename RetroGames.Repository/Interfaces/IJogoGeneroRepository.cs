using RetroGames.Model.Model;

namespace RetroGames.Repository.Interfaces;

public interface IJogoGeneroRepository
{
    // Lista todos os vínculos entre jogos e gêneros.
    Task<List<JogoGenero>> ListarTodosAsync();

    // Lista os vínculos dos jogos informados em uma única consulta.
    Task<List<JogoGenero>> ListarPorJogosAsync(List<int> idsJogos);

    // Insere os vínculos de um jogo com seus gêneros.
    Task InserirAsync(List<JogoGenero> vinculos);

    // Remove todos os vínculos de um jogo.
    Task RemoverPorJogoAsync(int idJogo);
}