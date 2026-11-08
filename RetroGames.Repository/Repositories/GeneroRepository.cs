using RetroGames.Model.Model;
using RetroGames.Repository.Interfaces;

namespace RetroGames.Repository.Repositories;

public class GeneroRepository : RepositoryBase, IGeneroRepository
{
    public GeneroRepository(RepositoryConfig config)
        : base(config)
    {
    }

    // Lista todos os gêneros.
    public async Task<List<Genero>> ListarAsync()
    {
        const string sql = """
            SELECT
                co_seq_genero,
                no_genero
            FROM tb_genero
            ORDER BY no_genero;
            """;

        var generos = new List<Genero>();

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        await using var reader = await comando.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            generos.Add(new Genero
            {
                Id = reader.GetInt32(0),
                Nome = reader.GetString(1)
            });
        }

        return generos;
    }
}