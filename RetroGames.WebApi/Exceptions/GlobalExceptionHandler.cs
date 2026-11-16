
using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Exceptions;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (status, responseCode, message) = exception switch
        {
            BusinessRuleException ex => (
                ex.Erro.Status,
                ObterResponseCode(ex.Erro),
                ex.Erro.Mensagem),

            _ => (
                HttpStatusCode.InternalServerError,
                ResponseCodes.ERRO,
                "Ocorreu um erro interno no servidor.")
        };

        if (status == HttpStatusCode.InternalServerError)
        {
            _logger.LogError(
                exception,
                "Ocorreu um erro não tratado durante a requisição.");
        }

        httpContext.Response.StatusCode = (int)status;

        var resposta = new ResponseApiModel
        {
            ResponseCode = responseCode,
            ErrorMessage = message
        };

        await httpContext.Response.WriteAsJsonAsync(
            resposta,
            cancellationToken);

        return true;
    }

    private static int ObterResponseCode(BusinessError erro)
    {
        if (erro == BusinessErrors.PlataformaNaoEncontrada)
            return ResponseCodes.PLATAFORMA_NAO_ENCONTRADA.Code;

        if (erro == BusinessErrors.PlataformaNomeDuplicado)
            return ResponseCodes.PLATAFORMA_NOME_DUPLICADO.Code;

        if (erro == BusinessErrors.PlataformaPossuiJogos)
            return ResponseCodes.PLATAFORMA_POSSUI_JOGOS.Code;

        if (erro == BusinessErrors.JogoNaoEncontrado)
            return ResponseCodes.JOGO_NAO_ENCONTRADO.Code;

        if (erro == BusinessErrors.CondicaoNaoEncontrada)
            return ResponseCodes.CONDICAO_INEXISTENTE.Code;

        if (erro == BusinessErrors.GeneroNaoEncontrado)
            return ResponseCodes.GENERO_INEXISTENTE.Code;

        return ResponseCodes.ERRO;
    }
}
