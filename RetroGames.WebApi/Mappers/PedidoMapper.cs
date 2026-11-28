using RetroGames.Model.Model;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Mappers;

public static class PedidoMapper
{
    public static List<ItemPedido> ParaModel(
        InserirPedidoRequestApiModel modelo)
    {
        // O preço não vem do front: o Service preenche com o valor do banco.
        return modelo.Itens
            .Select(item => new ItemPedido
            {
                IdJogo = item.IdJogo,
                Quantidade = item.Quantidade
            })
            .ToList();
    }

    public static InserirPedidoResponseApiModel ParaResponse(Pedido pedido)
    {
        return new InserirPedidoResponseApiModel
        {
            Id = pedido.Id,
            DataPedido = DateTime.SpecifyKind(
                pedido.DataPedido,
                DateTimeKind.Utc),
            Total = pedido.Total,
            IdSituacaoPedido = pedido.IdSituacaoPedido
        };
    }
}
