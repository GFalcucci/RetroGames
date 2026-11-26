using RetroGames.Services.ServiceModels;
using RetroGames.WebApi.ApiModels;
using RetroGames.Model.Model;
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

    public static Jogo ParaModel(InserirJogoRequestApiModel request)
    {
        return new Jogo
        {
            Nome = request.Nome,
            Descricao = string.IsNullOrWhiteSpace(request.Descricao)
                ? null
                : request.Descricao,
            AnoLancamento = request.AnoLancamento,
            Preco = request.Preco,
            Estoque = request.Estoque,
            IdPlataforma = request.IdPlataforma,
            IdCondicaoJogo = request.IdCondicaoJogo
        };
    }

    public static Jogo ParaModel(AlterarJogoRequestApiModel request)
    {
        return new Jogo
        {
            Nome = request.Nome,
            Descricao = string.IsNullOrWhiteSpace(request.Descricao)
                ? null
                : request.Descricao,
            AnoLancamento = request.AnoLancamento,
            Preco = request.Preco,
            Estoque = request.Estoque,
            IdPlataforma = request.IdPlataforma,
            IdCondicaoJogo = request.IdCondicaoJogo
        };
    }
}