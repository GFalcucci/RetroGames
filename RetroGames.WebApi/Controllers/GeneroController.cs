
using Microsoft.AspNetCore.Mvc;
using RetroGames.Services.Interfaces;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Mappers;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GeneroController : ControllerBase
{
    private readonly IGeneroService _generoService;

    public GeneroController(IGeneroService generoService)
    {
        _generoService = generoService;
    }

    [HttpGet]
    public async Task<ActionResult<ResponseApiModel<List<ListarGeneroResponseApiModel>>>> Listar()
    {
        var generos = await _generoService.ListarAsync();

        var generosResponse = generos
            .Select(GeneroMapper.ParaResponse)
            .ToList();

        var resposta = new ResponseApiModel<List<ListarGeneroResponseApiModel>>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = generosResponse,
            ErrorMessage = null
        };

        return Ok(resposta);
    }
}
