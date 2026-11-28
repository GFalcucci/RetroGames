using RetroGames.Model.Model;
using RetroGames.Repository.Interfaces;

namespace RetroGames.Repository.Repositories;

public class PedidoRepository : RepositoryBase, IPedidoRepository
{
    public PedidoRepository(RepositoryConfig config)
        : base(config)
    {
    }

    // 1. Insere um pedido e retorna o ID gerado pelo banco.
    public async Task<int> InserirAsync(Pedido pedido)
    {
        const string sql = """
            INSERT INTO tb_pedido (
                dt_pedido,
                vl_total,
                co_usuario,
                co_situacao_pedido
            )
            VALUES (
                @dataPedido,
                @total,
                @idUsuario,
                @idSituacaoPedido
            );
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@dataPedido", pedido.DataPedido);
        AdicionarParametro(comando, "@total", pedido.Total);
        AdicionarParametro(comando, "@idUsuario", pedido.IdUsuario);
        AdicionarParametro(comando, "@idSituacaoPedido", pedido.IdSituacaoPedido);

        await ExecutarGravacaoAsync(comando);

        return (int)comando.LastInsertedId;
    }
}
