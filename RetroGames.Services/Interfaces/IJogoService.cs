
using RetroGames.Services.ServiceModels;

namespace RetroGames.Services.Interfaces;

public interface IJogoService
{
    Task<(List<JogoDetalheServiceModel> Jogos, int Total)> ListarAsync(
        string? nome,
        int? idPlataforma,
        int? idGenero,
        int pagina,
        int tamanhoPagina);

    Task<JogoDetalheServiceModel> BuscarPorIdAsync(int id);

    Task<int> InserirAsync(JogoDetalheServiceModel jogo);

    Task AlterarAsync(JogoDetalheServiceModel jogo);

    Task DesativarAsync(int id);
}
