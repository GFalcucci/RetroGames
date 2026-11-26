
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetroGames.Services.Interfaces;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Configuration;
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

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<ResponseApiModel<PaginacaoResponseApiModel<ListarJogoResponseApiModel>>>> Listar(
        [FromQuery] ListarJogoRequestApiModel request)
    {
        var (jogos, total) = await _jogoService.ListarAsync(
            request.Busca,
            request.IdPlataforma,
            request.IdGenero,
            request.IncluirInativos,
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

    [AllowAnonymous]
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

    [Authorize(Policy = Politicas.Administrador)]
    [HttpPost]
    public async Task<ActionResult<ResponseApiModel<BuscarJogoPorIdResponseApiModel>>> Inserir(
        [FromBody] InserirJogoRequestApiModel request)
    {
        var id = await _jogoService.InserirAsync(
            JogoMapper.ParaModel(request),
            request.IdsGeneros);

        var criado = await _jogoService.BuscarPorIdAsync(id);

        var resposta = new ResponseApiModel<BuscarJogoPorIdResponseApiModel>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = JogoMapper.ParaBuscarPorId(criado),
            ErrorMessage = null
        };

        return CreatedAtAction(
            nameof(BuscarPorId),
            new { id },
            resposta);
    }

    [Authorize(Policy = Politicas.Administrador)]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ResponseApiModel<BuscarJogoPorIdResponseApiModel>>> Alterar(
        int id,
        [FromBody] AlterarJogoRequestApiModel request)
    {
        var jogo = JogoMapper.ParaModel(request);
        jogo.Id = id;

        await _jogoService.AlterarAsync(
            jogo,
            request.IdsGeneros);

        var alterado = await _jogoService.BuscarPorIdAsync(id);

        var resposta = new ResponseApiModel<BuscarJogoPorIdResponseApiModel>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = JogoMapper.ParaBuscarPorId(alterado),
            ErrorMessage = null
        };

        return Ok(resposta);
    }

    [Authorize(Policy = Politicas.Administrador)]
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ResponseApiModel<object?>>> Desativar(int id)
    {
        await _jogoService.DesativarAsync(id);

        var resposta = new ResponseApiModel<object?>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = null,
            ErrorMessage = null
        };

        return Ok(resposta);
    }
}
