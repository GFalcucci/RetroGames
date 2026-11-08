using RetroGames.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(new RepositoryConfig
{
    ConnectionString = builder.Configuration
        .GetConnectionString("Default")
        ?? throw new InvalidOperationException(
            "A connection string 'Default' não foi configurada.")
});

// Adiciona os serviços ao contêiner.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// Configura o pipeline de requisições HTTP.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();