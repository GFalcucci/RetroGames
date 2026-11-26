using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Configuration;
using RetroGames.WebApi.Filters;
using RetroGames.WebApi.Middlewares;

// Carrega as variáveis do arquivo .env.
DotNetEnv.Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

// Obtém a connection string configurada no ambiente.
var connectionString = builder.Configuration
    .GetConnectionString("Default");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "A connection string 'Default' não foi configurada. " +
        "Verifique o arquivo .env ou a configuração do ambiente.");
}

// Registra repositórios e serviços de negócio.
builder.Services.AddRepositorios(connectionString);
builder.Services.AddServicosDeNegocio();

// Registra os validators do projeto.
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Registra os recursos da API e o filtro de validação.
builder.Services.AddControllers(opcoes =>
{
    opcoes.Filters.Add<FluentValidationFilter>();
})
.ConfigureApiBehaviorOptions(opcoes =>
{
    opcoes.InvalidModelStateResponseFactory = contexto =>
    {
        var erros = contexto.ModelState
            .Where(item => item.Value?.Errors.Count > 0)
            .ToDictionary(
                item => JsonNamingPolicy.CamelCase.ConvertName(item.Key),
                item => item.Value!.Errors
                    .Select(erro =>
                        string.IsNullOrWhiteSpace(erro.ErrorMessage)
                            ? "Valor inválido."
                            : erro.ErrorMessage)
                    .ToArray());

        var resposta = new ResponseApiModel<Dictionary<string, string[]>>
        {
            ResponseCode = ResponseCodes.MODEL_STATE_INVALIDO.Code,
            Data = erros,
            ErrorMessage = ResponseCodes.MODEL_STATE_INVALIDO.DefaultMessage
        };

        return new BadRequestObjectResult(resposta);
    };
});

builder.Services.AddOpenApi();

// Registra o tratamento global de exceções.
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// O tratamento de exceções deve ser a primeira etapa do pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/openapi/v1.json",
            "RetroGames API v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();