using RetroGames.Model.Model;

namespace RetroGames.Repository.Interfaces;

public interface IJogoRepository
{
    // Lista os jogos com filtros opcionais e paginação.
    // Retorna a lista da página e o total de registros encontrados.
    Task<(List<Jogo> Jogos, int Total)> ListarAsync(
        string? nome,
        int? idPlataforma,
        List<int>? ids,
        bool incluirInativos,
        int pagina,
        int tamanhoPagina);

    // Busca um jogo pelo ID.
    Task<Jogo?> BuscarPorIdAsync(int id);

    // Insere um jogo e retorna o ID gerado pelo banco.
    Task<int> InserirAsync(Jogo jogo);

    // Altera os dados de um jogo.
    Task AlterarAsync(Jogo jogo);

    // Desativa um jogo, sem removê-lo do banco.
    Task DesativarAsync(int id);
}