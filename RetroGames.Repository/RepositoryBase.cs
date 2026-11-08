using MySqlConnector;

namespace RetroGames.Repository;

public abstract class RepositoryBase
{
    private readonly RepositoryConfig _config;

    protected RepositoryBase(RepositoryConfig config)
    {
        _config = config;
    }

    // 1. Abre uma conexão com o banco de dados.
    protected async Task<MySqlConnection> AbrirConexaoAsync()
    {
        var conexao = new MySqlConnection(_config.ConnectionString);

        try
        {
            await conexao.OpenAsync();
            return conexao;
        }
        catch
        {
            await conexao.DisposeAsync();
            throw;
        }
    }

    // 2. Cria um comando SQL associado a uma conexão.
    protected MySqlCommand CriarComando(
        MySqlConnection conexao,
        string sql)
    {
        return new MySqlCommand(sql, conexao);
    }

    // 3. Adiciona um parâmetro ao comando SQL.
    protected void AdicionarParametro(
        MySqlCommand comando,
        string nome,
        object? valor)
    {
        comando.Parameters.AddWithValue(
            nome,
            valor ?? DBNull.Value);
    }

    // 4. Executa INSERT ou UPDATE e trata valores duplicados.
    protected async Task<int> ExecutarGravacaoAsync(
        MySqlCommand comando)
    {
        try
        {
            return await comando.ExecuteNonQueryAsync();
        }
        catch (MySqlException ex) when (ex.Number == 1062)
        {
            throw new RegistroDuplicadoException(
                "Já existe um registro com esse valor único.");
        }
    }
}