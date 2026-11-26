
using RetroGames.Model.Model;
using RetroGames.Services.ServiceModels;

namespace RetroGames.Services.Interfaces;

public interface IJogoService
{
    Task<(List<JogoDetalheServiceModel> Jogos, int Total)> ListarAsync(
        string? nome,
        int? idPlataforma,
        int? idGenero,
        bool incluirInativos,
        int pagina,
        int tamanhoPagina);

    Task<JogoDetalheServiceModel> BuscarPorIdAsync(int id);

    Task<int> InserirAsync(Jogo jogo, List<int> idsGeneros);

    Task AlterarAsync(Jogo jogo, List<int> idsGeneros);

    Task DesativarAsync(int id);
}
