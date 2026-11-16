
using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Middlewares;

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
        int statusCode;
        int responseCode;
        string message;

        if (exception is BusinessRuleException ex)
        {
            statusCode = (int)ex.Erro.Status;
            responseCode = ex.Erro.Code;
            message = ex.Message;
        }
        else
        {
            _logger.LogError(
                exception,
                "Ocorreu um erro não tratado durante a requisição.");

            statusCode = StatusCodes.Status500InternalServerError;
            responseCode = ResponseCodes.ERRO;
            message = "Ocorreu um erro interno no servidor.";
        }

        httpContext.Response.StatusCode = statusCode;

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
}
