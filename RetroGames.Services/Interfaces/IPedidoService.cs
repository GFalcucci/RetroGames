using RetroGames.Model.Model;

namespace RetroGames.Services.Interfaces;

public interface IPedidoService
{
    // Cria o pedido do usuário logado: valida jogos e estoque, calcula o
    // total com o preço do banco e baixa o estoque em uma só transação.
    Task<Pedido> InserirAsync(List<ItemPedido> itens);
}
