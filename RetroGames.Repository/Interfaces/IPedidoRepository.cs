using RetroGames.Model.Model;

namespace RetroGames.Repository.Interfaces;

public interface IPedidoRepository
{
    // Insere um pedido e retorna o ID gerado pelo banco.
    Task<int> InserirAsync(Pedido pedido);
}
