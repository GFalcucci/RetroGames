using RetroGames.Model.Model;

namespace RetroGames.Repository.Interfaces;

public interface IItemPedidoRepository
{
    // Insere os itens de um pedido.
    Task InserirAsync(List<ItemPedido> itens);
}
