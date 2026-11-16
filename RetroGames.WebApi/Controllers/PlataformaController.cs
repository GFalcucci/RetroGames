
using Microsoft.AspNetCore.Mvc;
using RetroGames.Model.Model;
using RetroGames.Services.Interfaces;
using RetroGames.WebApi.ApiModels;

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
    public async Task<ActionResult<ResponseApiModel<List<Plataforma>>>> Listar()
    {
        var plataformas = await _plataformaService.ListarAsync();

        var resposta = new ResponseApiModel<List<Plataforma>>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = plataformas,
            ErrorMessage = null
        };

        return Ok(resposta);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ResponseApiModel<Plataforma>>> BuscarPorId(int id)
    {
        var plataforma = await _plataformaService.BuscarPorIdAsync(id);

        var resposta = new ResponseApiModel<Plataforma>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = plataforma,
            ErrorMessage = null
        };

        return Ok(resposta);
    }
}
