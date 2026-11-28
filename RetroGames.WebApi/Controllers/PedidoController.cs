using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetroGames.Services.Interfaces;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Configuration;
using RetroGames.WebApi.Mappers;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PedidoController : ControllerBase
{
    private readonly IPedidoService _pedidoService;

    public PedidoController(IPedidoService pedidoService)
    {
        _pedidoService = pedidoService;
    }

    [Authorize(Policy = Politicas.UsuarioAutenticado)]
    [HttpPost]
    public async Task<ActionResult<ResponseApiModel<InserirPedidoResponseApiModel>>> Inserir(
        InserirPedidoRequestApiModel modelo)
    {
        var itens = PedidoMapper.ParaModel(modelo);

        var pedido = await _pedidoService.InserirAsync(itens);

        var resposta = new ResponseApiModel<InserirPedidoResponseApiModel>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = PedidoMapper.ParaResponse(pedido),
            ErrorMessage = null
        };

        return StatusCode(StatusCodes.Status201Created, resposta);
    }
}
