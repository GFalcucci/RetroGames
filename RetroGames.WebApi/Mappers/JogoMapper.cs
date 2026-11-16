using RetroGames.Services.ServiceModels;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Mappers;

public static class JogoMapper
{
    public static ListarJogoResponseApiModel ParaListar(
        JogoDetalheServiceModel jogo)
    {
        return new ListarJogoResponseApiModel
        {
            Id = jogo.Jogo.Id,
            Nome = jogo.Jogo.Nome,
            AnoLancamento = jogo.Jogo.AnoLancamento,
            Preco = jogo.Jogo.Preco,
            Estoque = jogo.Jogo.Estoque,
            NomePlataforma = jogo.NomePlataforma,
            DescricaoCondicao = jogo.DescricaoCondicao,
            Generos = jogo.Generos
                .Select(genero => genero.Nome)
                .ToList()
        };
    }

    public static BuscarJogoPorIdResponseApiModel ParaBuscarPorId(
        JogoDetalheServiceModel jogo)
    {
        return new BuscarJogoPorIdResponseApiModel
        {
            Id = jogo.Jogo.Id,
            Nome = jogo.Jogo.Nome,
            Descricao = jogo.Jogo.Descricao,
            AnoLancamento = jogo.Jogo.AnoLancamento,
            Preco = jogo.Jogo.Preco,
            Estoque = jogo.Jogo.Estoque,
            NomePlataforma = jogo.NomePlataforma,
            DescricaoCondicao = jogo.DescricaoCondicao,
            Generos = jogo.Generos
                .Select(genero => genero.Nome)
                .ToList()
        };
    }
}