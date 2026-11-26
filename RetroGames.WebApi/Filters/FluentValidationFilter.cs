
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Filters;

public class FluentValidationFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        var erros = new Dictionary<string, List<string>>();

        foreach (var argumento in context.ActionArguments.Values)
        {
            if (argumento is null)
                continue;

            var tipoValidator = typeof(IValidator<>)
                .MakeGenericType(argumento.GetType());

            var validator = context.HttpContext.RequestServices
                .GetService(tipoValidator) as IValidator;

            if (validator is null)
                continue;

            var validationContext = new ValidationContext<object>(argumento);

            var resultado = await validator.ValidateAsync(
                validationContext,
                context.HttpContext.RequestAborted);

            foreach (var erro in resultado.Errors)
            {
                var nomeCampo = JsonNamingPolicy.CamelCase
                    .ConvertName(erro.PropertyName);

                if (!erros.ContainsKey(nomeCampo))
                    erros[nomeCampo] = new List<string>();

                erros[nomeCampo].Add(erro.ErrorMessage);
            }
        }

        if (erros.Count > 0)
        {
            var resposta = new ResponseApiModel<Dictionary<string, string[]>>
            {
                ResponseCode = ResponseCodes.MODEL_STATE_INVALIDO.Code,
                Data = erros.ToDictionary(
                    item => item.Key,
                    item => item.Value.ToArray()),
                ErrorMessage = ResponseCodes.MODEL_STATE_INVALIDO.DefaultMessage
            };

            context.Result = new BadRequestObjectResult(resposta);
            return;
        }

        await next();
    }
}
