using RetroGames.Model.Model;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Mappers;

public static class PlataformaMapper
{
    public static ListarPlataformaResponseApiModel ParaResponse(
        Plataforma plataforma)
    {
        return new ListarPlataformaResponseApiModel
        {
            Id = plataforma.Id,
            Nome = plataforma.Nome,
            Fabricante = plataforma.Fabricante,
            AnoLancamento = plataforma.AnoLancamento
        };
    }

    public static Plataforma ParaModel(
        InserirPlataformaRequestApiModel modelo)
    {
        return new Plataforma
        {
            Nome = modelo.Nome,
            Fabricante = modelo.Fabricante,
            AnoLancamento = modelo.AnoLancamento
        };
    }

    public static Plataforma ParaModel(
        AlterarPlataformaRequestApiModel modelo)
    {
        return new Plataforma
        {
            Nome = modelo.Nome,
            Fabricante = modelo.Fabricante,
            AnoLancamento = modelo.AnoLancamento
        };
    }
}