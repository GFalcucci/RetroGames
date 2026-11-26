
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetroGames.Services.Interfaces;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Configuration;
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

    [AllowAnonymous]
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

    [AllowAnonymous]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ResponseApiModel<ListarPlataformaResponseApiModel>>> ListarPorId(int id)
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

    [Authorize(Policy = Politicas.Administrador)]
    [HttpPost]
    public async Task<ActionResult<ResponseApiModel<ListarPlataformaResponseApiModel>>> Inserir(
        InserirPlataformaRequestApiModel modelo)
    {
        var plataforma = PlataformaMapper.ParaModel(modelo);

        var id = await _plataformaService.InserirAsync(plataforma);

        var criada = await _plataformaService.BuscarPorIdAsync(id);

        var resposta = new ResponseApiModel<ListarPlataformaResponseApiModel>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = PlataformaMapper.ParaResponse(criada),
            ErrorMessage = null
        };

        return CreatedAtAction(nameof(ListarPorId), new { id = criada.Id }, resposta);
    }

    [Authorize(Policy = Politicas.Administrador)]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ResponseApiModel<ListarPlataformaResponseApiModel>>> Alterar(
        int id,
        AlterarPlataformaRequestApiModel modelo)
    {
        var plataforma = PlataformaMapper.ParaModel(modelo);
        plataforma.Id = id;

        await _plataformaService.AlterarAsync(plataforma);

        var alterada = await _plataformaService.BuscarPorIdAsync(id);

        var resposta = new ResponseApiModel<ListarPlataformaResponseApiModel>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = PlataformaMapper.ParaResponse(alterada),
            ErrorMessage = null
        };

        return Ok(resposta);
    }

    [Authorize(Policy = Politicas.Administrador)]
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ResponseApiModel<object?>>> Excluir(int id)
    {
        await _plataformaService.ExcluirAsync(id);

        var resposta = new ResponseApiModel<object?>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = null,
            ErrorMessage = null
        };

        return Ok(resposta);
    }
}
