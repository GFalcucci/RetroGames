namespace RetroGames.WebApi.ApiModels;

public class BuscarJogoPorIdResponseApiModel
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string? Descricao { get; set; }

    public int AnoLancamento { get; set; }

    public decimal Preco { get; set; }

    public int Estoque { get; set; }

    public int IdPlataforma { get; set; }

    public int IdCondicaoJogo { get; set; }

    public List<int> IdsGeneros { get; set; } = [];

    public string NomePlataforma { get; set; } = string.Empty;

    public string DescricaoCondicao { get; set; } = string.Empty;

    public List<string> Generos { get; set; } = [];
}