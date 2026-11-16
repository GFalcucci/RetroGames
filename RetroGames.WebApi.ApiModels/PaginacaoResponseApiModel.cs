namespace RetroGames.WebApi.ApiModels;

public class PaginacaoResponseApiModel<T>
{
    public List<T> Itens { get; set; } = [];

    public int Total { get; set; }

    public int Pagina { get; set; }

    public int TamanhoPagina { get; set; }

    public int TotalPaginas { get; set; }
}