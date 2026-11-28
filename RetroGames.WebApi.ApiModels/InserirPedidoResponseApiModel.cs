namespace RetroGames.WebApi.ApiModels;

public class InserirPedidoResponseApiModel
{
    public int Id { get; set; }
    public DateTime DataPedido { get; set; }
    public decimal Total { get; set; }
    public int IdSituacaoPedido { get; set; }
}
