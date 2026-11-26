
using MySqlConnector;
using RetroGames.Model.Model;
using RetroGames.Repository.Interfaces;

namespace RetroGames.Repository.Repositories;

public class UsuarioRepository : RepositoryBase, IUsuarioRepository
{
    public UsuarioRepository(RepositoryConfig config)
        : base(config)
    {
    }

    public async Task<Usuario?> BuscarPorIdAsync(int id)
    {
        const string sql = @"
            SELECT
                co_seq_usuario,
                no_usuario,
                ds_email,
                ds_senha_hash,
                st_ativo,
                dt_cadastro,
                co_perfil
            FROM tb_usuario
            WHERE co_seq_usuario = @id;";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@id", id);

        await using var reader = await comando.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            return null;

        return MapearUsuario(reader);
    }

    public async Task<Usuario?> BuscarPorEmailAsync(string email)
    {
        const string sql = @"
            SELECT
                co_seq_usuario,
                no_usuario,
                ds_email,
                ds_senha_hash,
                st_ativo,
                dt_cadastro,
                co_perfil
            FROM tb_usuario
            WHERE ds_email = @email;";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@email", email);

        await using var reader = await comando.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            return null;

        return MapearUsuario(reader);
    }

    public async Task<bool> ExisteComPerfilAsync(int idPerfil)
    {
        const string sql = @"
            SELECT EXISTS (
                SELECT 1
                FROM tb_usuario
                WHERE co_perfil = @idPerfil
            );";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@idPerfil", idPerfil);

        var resultado = await comando.ExecuteScalarAsync();

        return Convert.ToInt32(resultado) == 1;
    }

    public async Task<int> InserirAsync(Usuario usuario)
    {
        const string sql = @"
            INSERT INTO tb_usuario (
                no_usuario,
                ds_email,
                ds_senha_hash,
                st_ativo,
                dt_cadastro,
                co_perfil
            )
            VALUES (
                @nome,
                @email,
                @senhaHash,
                @ativo,
                @dataCadastro,
                @idPerfil
            );";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@nome", usuario.Nome);
        AdicionarParametro(comando, "@email", usuario.Email);
        AdicionarParametro(comando, "@senhaHash", usuario.SenhaHash);
        AdicionarParametro(comando, "@ativo", usuario.Ativo);
        AdicionarParametro(comando, "@dataCadastro", usuario.DataCadastro);
        AdicionarParametro(comando, "@idPerfil", usuario.IdPerfil);

        await ExecutarGravacaoAsync(comando);

        return Convert.ToInt32(comando.LastInsertedId);
    }

    private static Usuario MapearUsuario(MySqlDataReader reader)
    {
        return new Usuario
        {
            Id = reader.GetInt32(0),
            Nome = reader.GetString(1),
            Email = reader.GetString(2),
            SenhaHash = reader.GetString(3),
            Ativo = reader.GetBoolean(4),
            DataCadastro = reader.GetDateTime(5),
            IdPerfil = reader.GetInt32(6)
        };
    }
}
