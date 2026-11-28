using RetroGames.Model.Model;
using RetroGames.Repository.Interfaces;

namespace RetroGames.Repository.Repositories;

public class ItemPedidoRepository : RepositoryBase, IItemPedidoRepository
{
    public ItemPedidoRepository(RepositoryConfig config)
        : base(config)
    {
    }

    // 1. Insere os itens de um pedido.
    public async Task InserirAsync(List<ItemPedido> itens)
    {
        if (itens.Count == 0)
            return;

        const string sql = """
            INSERT INTO tb_item_pedido (
                qt_item,
                vl_unitario,
                co_pedido,
                co_jogo
            )
            VALUES (
                @quantidade,
                @precoUnitario,
                @idPedido,
                @idJogo
            );
            """;

        await using var conexao = await AbrirConexaoAsync();

        foreach (var item in itens)
        {
            await using var comando = CriarComando(conexao, sql);

            AdicionarParametro(comando, "@quantidade", item.Quantidade);
            AdicionarParametro(comando, "@precoUnitario", item.PrecoUnitario);
            AdicionarParametro(comando, "@idPedido", item.IdPedido);
            AdicionarParametro(comando, "@idJogo", item.IdJogo);

            await ExecutarGravacaoAsync(comando);
        }
    }
}
