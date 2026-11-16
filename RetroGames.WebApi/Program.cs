
using RetroGames.WebApi.Configuration;
using RetroGames.WebApi.Exceptions;

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

// Registra os recursos da API.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Registra o tratamento global de exceções.
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Ativa o tratamento global de exceções.
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
