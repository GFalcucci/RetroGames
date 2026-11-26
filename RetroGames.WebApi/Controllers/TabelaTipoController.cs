
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetroGames.Services.Interfaces;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Mappers;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TabelaTipoController : ControllerBase
{
    private readonly ICondicaoJogoService _condicaoJogoService;

    public TabelaTipoController(
        ICondicaoJogoService condicaoJogoService)
    {
        _condicaoJogoService = condicaoJogoService;
    }

    [AllowAnonymous]
    [HttpGet("condicoes-jogo")]
    public async Task<ActionResult<ResponseApiModel<List<ListarCondicaoJogoResponseApiModel>>>> Listar()
    {
        var condicoes = await _condicaoJogoService.ListarAsync();

        var condicoesResponse = condicoes
            .Select(CondicaoJogoMapper.ParaResponse)
            .ToList();

        var resposta = new ResponseApiModel<List<ListarCondicaoJogoResponseApiModel>>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = condicoesResponse,
            ErrorMessage = null
        };

        return Ok(resposta);
    }
}
