
using Microsoft.AspNetCore.Mvc;
using RetroGames.Services.Interfaces;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Mappers;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JogoController : ControllerBase
{
    private readonly IJogoService _jogoService;

    public JogoController(IJogoService jogoService)
    {
        _jogoService = jogoService;
    }

    [HttpGet]
    public async Task<ActionResult<ResponseApiModel<PaginacaoResponseApiModel<ListarJogoResponseApiModel>>>> Listar(
        [FromQuery] ListarJogoRequestApiModel request)
    {
        var (jogos, total) = await _jogoService.ListarAsync(
            request.Busca,
            request.IdPlataforma,
            request.IdGenero,
            request.Pagina,
            request.TamanhoPagina);

        var jogosResponse = jogos
            .Select(JogoMapper.ParaListar)
            .ToList();

        var paginacao = new PaginacaoResponseApiModel<ListarJogoResponseApiModel>
        {
            Itens = jogosResponse,
            Total = total,
            Pagina = request.Pagina,
            TamanhoPagina = request.TamanhoPagina,
            TotalPaginas = request.TamanhoPagina > 0
                ? (int)Math.Ceiling((double)total / request.TamanhoPagina)
                : 0
        };

        var resposta = new ResponseApiModel<PaginacaoResponseApiModel<ListarJogoResponseApiModel>>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = paginacao,
            ErrorMessage = null
        };

        return Ok(resposta);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ResponseApiModel<BuscarJogoPorIdResponseApiModel>>> BuscarPorId(
        int id)
    {
        var jogo = await _jogoService.BuscarPorIdAsync(id);

        var jogoResponse = JogoMapper.ParaBuscarPorId(jogo);

        var resposta = new ResponseApiModel<BuscarJogoPorIdResponseApiModel>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = jogoResponse,
            ErrorMessage = null
        };

        return Ok(resposta);
    }
}
