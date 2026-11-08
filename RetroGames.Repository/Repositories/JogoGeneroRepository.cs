using MySqlConnector;
using RetroGames.Model.Model;
using RetroGames.Repository.Interfaces;

namespace RetroGames.Repository.Repositories;

public class JogoGeneroRepository : RepositoryBase, IJogoGeneroRepository
{
    public JogoGeneroRepository(RepositoryConfig config)
        : base(config)
    {
    }

    // 1. Lista todos os vínculos entre jogos e gêneros.
    public async Task<List<JogoGenero>> ListarTodosAsync()
    {
        const string sql = """
            SELECT
                co_jogo,
                co_genero
            FROM tb_jogo_genero
            ORDER BY co_jogo, co_genero;
            """;

        var vinculos = new List<JogoGenero>();

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        await using var reader = await comando.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            vinculos.Add(new JogoGenero
            {
                IdJogo = reader.GetInt32(0),
                IdGenero = reader.GetInt32(1)
            });
        }

        return vinculos;
    }

    // 2. Lista os vínculos dos jogos informados em uma única consulta.
    public async Task<List<JogoGenero>> ListarPorJogosAsync(List<int> idsJogos)
    {
        if (idsJogos.Count == 0)
            return new List<JogoGenero>();

        var parametrosIds = idsJogos
            .Select((_, indice) => $"@id{indice}")
            .ToList();

        var sql = $"""
            SELECT
                co_jogo,
                co_genero
            FROM tb_jogo_genero
            WHERE co_jogo IN ({string.Join(", ", parametrosIds)})
            ORDER BY co_jogo, co_genero;
            """;

        var vinculos = new List<JogoGenero>();

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        for (int i = 0; i < idsJogos.Count; i++)
        {
            AdicionarParametro(comando, $"@id{i}", idsJogos[i]);
        }

        await using var reader = await comando.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            vinculos.Add(new JogoGenero
            {
                IdJogo = reader.GetInt32(0),
                IdGenero = reader.GetInt32(1)
            });
        }

        return vinculos;
    }

    // 3. Insere os vínculos de um jogo com seus gêneros.
    public async Task InserirAsync(List<JogoGenero> vinculos)
    {
        if (vinculos.Count == 0)
            return;

        const string sql = """
            INSERT INTO tb_jogo_genero (
                co_jogo,
                co_genero
            )
            VALUES (
                @idJogo,
                @idGenero
            );
            """;

        await using var conexao = await AbrirConexaoAsync();

        foreach (var vinculo in vinculos)
        {
            await using var comando = CriarComando(conexao, sql);

            AdicionarParametro(comando, "@idJogo", vinculo.IdJogo);
            AdicionarParametro(comando, "@idGenero", vinculo.IdGenero);

            await ExecutarGravacaoAsync(comando);
        }
    }

    // 4. Remove todos os vínculos de um jogo.
    public async Task RemoverPorJogoAsync(int idJogo)
    {
        const string sql = """
            DELETE FROM tb_jogo_genero
            WHERE co_jogo = @idJogo;
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@idJogo", idJogo);

        await comando.ExecuteNonQueryAsync();
    }
}