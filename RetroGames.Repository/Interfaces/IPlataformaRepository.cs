using RetroGames.Model.Model;

namespace RetroGames.Repository.Interfaces;

public interface IPlataformaRepository
{
    // Lista todas as plataformas ordenadas por nome.
    Task<List<Plataforma>> ListarAsync();

    // Busca uma plataforma pelo ID.
    Task<Plataforma?> BuscarPorIdAsync(int id);

    // Verifica se já existe uma plataforma com o nome informado.
    // O ID opcional permite ignorar a própria plataforma durante uma alteração.
    Task<bool> ExisteComNomeAsync(string nome, int? idIgnorado = null);

    // Verifica se existem jogos vinculados à plataforma.
    Task<bool> PossuiJogosAsync(int idPlataforma);

    // Insere uma plataforma e retorna o ID gerado pelo banco.
    Task<int> InserirAsync(Plataforma plataforma);

    // Altera os dados de uma plataforma.
    Task AlterarAsync(Plataforma plataforma);

    // Exclui uma plataforma pelo ID.
    Task ExcluirAsync(int id);
}