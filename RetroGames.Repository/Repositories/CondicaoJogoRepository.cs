using RetroGames.Model.Model;
using RetroGames.Repository.Interfaces;

namespace RetroGames.Repository.Repositories;

public class CondicaoJogoRepository : RepositoryBase, ICondicaoJogoRepository
{
    public CondicaoJogoRepository(RepositoryConfig config)
        : base(config)
    {
    }

    // Lista todas as condições de jogo.
    public async Task<List<CondicaoJogo>> ListarAsync()
    {
        const string sql = """
            SELECT
                co_condicao_jogo,
                ds_condicao_jogo
            FROM tb_condicao_jogo
            ORDER BY ds_condicao_jogo;
            """;

        var condicoes = new List<CondicaoJogo>();

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        await using var reader = await comando.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            condicoes.Add(new CondicaoJogo
            {
                Id = reader.GetInt32(0),
                Descricao = reader.GetString(1)
            });
        }

        return condicoes;
    }
}