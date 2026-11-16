using Microsoft.AspNetCore.Mvc;
using RetroGames.Services.Interfaces;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Mappers;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlataformaController : ControllerBase
{
    private readonly IPlataformaService _plataformaService;

    public PlataformaController(IPlataformaService plataformaService)
    {
        _plataformaService = plataformaService;
    }

    [HttpGet]
    public async Task<ActionResult<ResponseApiModel<List<ListarPlataformaResponseApiModel>>>> Listar()
    {
        var plataformas = await _plataformaService.ListarAsync();

        var plataformasResponse = plataformas
            .Select(PlataformaMapper.ParaResponse)
            .ToList();

        var resposta = new ResponseApiModel<List<ListarPlataformaResponseApiModel>>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = plataformasResponse,
            ErrorMessage = null
        };

        return Ok(resposta);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ResponseApiModel<ListarPlataformaResponseApiModel>>> BuscarPorId(int id)
    {
        var plataforma = await _plataformaService.BuscarPorIdAsync(id);

        var plataformaResponse = PlataformaMapper.ParaResponse(plataforma);

        var resposta = new ResponseApiModel<ListarPlataformaResponseApiModel>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = plataformaResponse,
            ErrorMessage = null
        };

        return Ok(resposta);
    }
}