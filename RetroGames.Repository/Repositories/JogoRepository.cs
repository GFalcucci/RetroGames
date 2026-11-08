using RetroGames.Model.Model;
using RetroGames.Repository.Interfaces;

namespace RetroGames.Repository.Repositories;

public class JogoRepository : RepositoryBase, IJogoRepository
{
    public JogoRepository(RepositoryConfig config)
        : base(config)
    {
    }

    // 1. Lista os jogos com filtros opcionais e paginação.
    // Também retorna o total de jogos encontrados com os mesmos filtros.
    public async Task<(List<Jogo> Jogos, int Total)> ListarAsync(
        string? nome,
        int? idPlataforma,
        List<int>? ids,
        bool incluirInativos,
        int pagina,
        int tamanhoPagina)
    {
        var jogos = new List<Jogo>();

        var condicoes = new List<string>();

        if (!incluirInativos)
            condicoes.Add("st_ativo = 1");

        if (!string.IsNullOrWhiteSpace(nome))
            condicoes.Add("no_jogo LIKE @nome");

        if (idPlataforma.HasValue)
            condicoes.Add("co_plataforma = @idPlataforma");

        if (ids != null && ids.Count > 0)
        {
            var parametrosIds = ids
                .Select((_, indice) => $"@id{indice}")
                .ToList();

            condicoes.Add(
                $"co_seq_jogo IN ({string.Join(", ", parametrosIds)})");
        }

        var where = condicoes.Count > 0
            ? "WHERE " + string.Join(" AND ", condicoes)
            : string.Empty;

        var sqlLista = $"""
            SELECT
                co_seq_jogo,
                no_jogo,
                ds_jogo,
                nu_ano_lancamento,
                vl_preco,
                qt_estoque,
                st_ativo,
                dt_cadastro,
                co_plataforma,
                co_condicao_jogo
            FROM tb_jogo
            {where}
            ORDER BY no_jogo
            LIMIT @limite OFFSET @offset;
            """;

        var sqlContagem = $"""
            SELECT COUNT(*)
            FROM tb_jogo
            {where};
            """;

        await using var conexao = await AbrirConexaoAsync();

        // Executa a consulta da página.
        await using (var comando = CriarComando(conexao, sqlLista))
        {
            AdicionarFiltros(comando, nome, idPlataforma, ids);

            comando.Parameters.AddWithValue("@limite", tamanhoPagina);
            comando.Parameters.AddWithValue(
                "@offset",
                (pagina - 1) * tamanhoPagina);

            await using var reader = await comando.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                jogos.Add(new Jogo
                {
                    Id = reader.GetInt32(0),
                    Nome = reader.GetString(1),
                    Descricao = reader.IsDBNull(2)
                        ? null
                        : reader.GetString(2),
                    AnoLancamento = reader.GetInt32(3),
                    Preco = reader.GetDecimal(4),
                    Estoque = reader.GetInt32(5),
                    Ativo = reader.GetBoolean(6),
                    DataCadastro = reader.GetDateTime(7),
                    IdPlataforma = reader.GetInt32(8),
                    IdCondicaoJogo = reader.GetInt32(9)
                });
            }
        }

        // Executa a contagem usando exatamente os mesmos filtros.
        int total;

        await using (var comando = CriarComando(conexao, sqlContagem))
        {
            AdicionarFiltros(comando, nome, idPlataforma, ids);

            var resultado = await comando.ExecuteScalarAsync();
            total = Convert.ToInt32(resultado);
        }

        return (jogos, total);
    }

    // 2. Busca um jogo pelo ID.
    public async Task<Jogo?> BuscarPorIdAsync(int id)
    {
        const string sql = """
            SELECT
                co_seq_jogo,
                no_jogo,
                ds_jogo,
                nu_ano_lancamento,
                vl_preco,
                qt_estoque,
                st_ativo,
                dt_cadastro,
                co_plataforma,
                co_condicao_jogo
            FROM tb_jogo
            WHERE co_seq_jogo = @id;
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@id", id);

        await using var reader = await comando.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            return null;

        return new Jogo
        {
            Id = reader.GetInt32(0),
            Nome = reader.GetString(1),
            Descricao = reader.IsDBNull(2)
                ? null
                : reader.GetString(2),
            AnoLancamento = reader.GetInt32(3),
            Preco = reader.GetDecimal(4),
            Estoque = reader.GetInt32(5),
            Ativo = reader.GetBoolean(6),
            DataCadastro = reader.GetDateTime(7),
            IdPlataforma = reader.GetInt32(8),
            IdCondicaoJogo = reader.GetInt32(9)
        };
    }

    // 3. Insere um jogo e retorna o ID gerado pelo banco.
    public async Task<int> InserirAsync(Jogo jogo)
    {
        const string sql = """
            INSERT INTO tb_jogo (
                no_jogo,
                ds_jogo,
                nu_ano_lancamento,
                vl_preco,
                qt_estoque,
                st_ativo,
                dt_cadastro,
                co_plataforma,
                co_condicao_jogo
            )
            VALUES (
                @nome,
                @descricao,
                @anoLancamento,
                @preco,
                @estoque,
                @ativo,
                @dataCadastro,
                @idPlataforma,
                @idCondicaoJogo
            );
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@nome", jogo.Nome);
        AdicionarParametro(comando, "@descricao", jogo.Descricao);
        AdicionarParametro(comando, "@anoLancamento", jogo.AnoLancamento);
        AdicionarParametro(comando, "@preco", jogo.Preco);
        AdicionarParametro(comando, "@estoque", jogo.Estoque);
        AdicionarParametro(comando, "@ativo", jogo.Ativo);
        AdicionarParametro(comando, "@dataCadastro", jogo.DataCadastro);
        AdicionarParametro(comando, "@idPlataforma", jogo.IdPlataforma);
        AdicionarParametro(comando, "@idCondicaoJogo", jogo.IdCondicaoJogo);

        await ExecutarGravacaoAsync(comando);

        return (int)comando.LastInsertedId;
    }

    // 4. Altera os dados de um jogo.
    public async Task AlterarAsync(Jogo jogo)
    {
        const string sql = """
            UPDATE tb_jogo
            SET
                no_jogo = @nome,
                ds_jogo = @descricao,
                nu_ano_lancamento = @anoLancamento,
                vl_preco = @preco,
                qt_estoque = @estoque,
                co_plataforma = @idPlataforma,
                co_condicao_jogo = @idCondicaoJogo
            WHERE co_seq_jogo = @id;
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@nome", jogo.Nome);
        AdicionarParametro(comando, "@descricao", jogo.Descricao);
        AdicionarParametro(comando, "@anoLancamento", jogo.AnoLancamento);
        AdicionarParametro(comando, "@preco", jogo.Preco);
        AdicionarParametro(comando, "@estoque", jogo.Estoque);
        AdicionarParametro(comando, "@idPlataforma", jogo.IdPlataforma);
        AdicionarParametro(comando, "@idCondicaoJogo", jogo.IdCondicaoJogo);
        AdicionarParametro(comando, "@id", jogo.Id);

        await ExecutarGravacaoAsync(comando);
    }

    // 5. Desativa um jogo sem removê-lo do banco.
    public async Task DesativarAsync(int id)
    {
        const string sql = """
            UPDATE tb_jogo
            SET st_ativo = 0
            WHERE co_seq_jogo = @id;
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@id", id);

        await comando.ExecuteNonQueryAsync();
    }

    // Adiciona os parâmetros dos filtros às consultas.
    private void AdicionarFiltros(
        MySqlConnector.MySqlCommand comando,
        string? nome,
        int? idPlataforma,
        List<int>? ids)
    {
        if (!string.IsNullOrWhiteSpace(nome))
            AdicionarParametro(comando, "@nome", $"%{nome}%");

        if (idPlataforma.HasValue)
            AdicionarParametro(comando, "@idPlataforma", idPlataforma.Value);

        if (ids != null && ids.Count > 0)
        {
            for (int i = 0; i < ids.Count; i++)
                AdicionarParametro(comando, $"@id{i}", ids[i]);
        }
    }
}