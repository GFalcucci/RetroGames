using RetroGames.Model.Model;
using RetroGames.Repository.Interfaces;

namespace RetroGames.Repository.Repositories;

public class PlataformaRepository : RepositoryBase, IPlataformaRepository
{
    public PlataformaRepository(RepositoryConfig config)
        : base(config)
    {
    }

    // 1. Lista todas as plataformas ordenadas por nome.
    public async Task<List<Plataforma>> ListarAsync()
    {
        const string sql = @"
            SELECT
                co_seq_plataforma,
                no_plataforma,
                no_fabricante,
                nu_ano_lancamento
            FROM tb_plataforma
            ORDER BY no_plataforma;";

        var plataformas = new List<Plataforma>();

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        await using var reader = await comando.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            plataformas.Add(new Plataforma
            {
                Id = reader.GetInt32(0),
                Nome = reader.GetString(1),
                Fabricante = reader.GetString(2),
                AnoLancamento = reader.GetInt32(3)
            });
        }

        return plataformas;
    }

    // 2. Busca uma plataforma pelo ID.
    public async Task<Plataforma?> BuscarPorIdAsync(int id)
    {
        const string sql = @"
            SELECT
                co_seq_plataforma,
                no_plataforma,
                no_fabricante,
                nu_ano_lancamento
            FROM tb_plataforma
            WHERE co_seq_plataforma = @id;";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@id", id);

        await using var reader = await comando.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            return null;

        return new Plataforma
        {
            Id = reader.GetInt32(0),
            Nome = reader.GetString(1),
            Fabricante = reader.GetString(2),
            AnoLancamento = reader.GetInt32(3)
        };
    }

    // 3. Verifica se já existe uma plataforma com o nome informado.
    public async Task<bool> ExisteComNomeAsync(string nome, int? idIgnorado = null)
    {
        const string sql = @"
            SELECT EXISTS (
                SELECT 1
                FROM tb_plataforma
                WHERE no_plataforma = @nome
                  AND (@idIgnorado IS NULL
                       OR co_seq_plataforma <> @idIgnorado)
            );";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@nome", nome);
        AdicionarParametro(comando, "@idIgnorado", idIgnorado);

        var resultado = await comando.ExecuteScalarAsync();

        return Convert.ToInt32(resultado) == 1;
    }

    // 4. Verifica se existem jogos vinculados à plataforma.
    public async Task<bool> PossuiJogosAsync(int idPlataforma)
    {
        const string sql = @"
            SELECT EXISTS (
                SELECT 1
                FROM tb_jogo
                WHERE co_plataforma = @idPlataforma
            );";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@idPlataforma", idPlataforma);

        var resultado = await comando.ExecuteScalarAsync();

        return Convert.ToInt32(resultado) == 1;
    }

    // 5. Insere uma plataforma e retorna o ID gerado pelo banco.
    public async Task<int> InserirAsync(Plataforma plataforma)
    {
        const string sql = @"
            INSERT INTO tb_plataforma (
                no_plataforma,
                no_fabricante,
                nu_ano_lancamento
            )
            VALUES (
                @nome,
                @fabricante,
                @anoLancamento
            );";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@nome", plataforma.Nome);
        AdicionarParametro(comando, "@fabricante", plataforma.Fabricante);
        AdicionarParametro(comando, "@anoLancamento", plataforma.AnoLancamento);

        await ExecutarGravacaoAsync(comando);

        return Convert.ToInt32(comando.LastInsertedId);
    }

    // 6. Altera os dados de uma plataforma.
    public async Task AlterarAsync(Plataforma plataforma)
    {
        const string sql = @"
            UPDATE tb_plataforma
            SET
                no_plataforma = @nome,
                no_fabricante = @fabricante,
                nu_ano_lancamento = @anoLancamento
            WHERE co_seq_plataforma = @id;";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@id", plataforma.Id);
        AdicionarParametro(comando, "@nome", plataforma.Nome);
        AdicionarParametro(comando, "@fabricante", plataforma.Fabricante);
        AdicionarParametro(comando, "@anoLancamento", plataforma.AnoLancamento);

        await ExecutarGravacaoAsync(comando);
    }

    // 7. Exclui uma plataforma pelo ID.
    public async Task ExcluirAsync(int id)
    {
        const string sql = @"
            DELETE FROM tb_plataforma
            WHERE co_seq_plataforma = @id;";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@id", id);

        await comando.ExecuteNonQueryAsync();
    }
}