
using Moq;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Repository.Interfaces;
using RetroGames.Services;
using Xunit;

namespace RetroGames.WebApi.UnitTests;

public class GeneroServiceTests
{
    private readonly Mock<IRepositorySet> _repositorioMock = new();
    private readonly Mock<IGeneroRepository> _generoRepositorioMock = new();

    private readonly GeneroService _servico;

    public GeneroServiceTests()
    {
        _repositorioMock
            .SetupGet(r => r.Genero)
            .Returns(_generoRepositorioMock.Object);

        _servico = new GeneroService(_repositorioMock.Object);
    }

    [Fact]
    public async Task ListarAsync_DeveRetornarGenerosDoRepositorio()
    {
        // Arrange
        var generosEsperados = new List<Genero>
        {
            new() { Id = 1, Nome = "Ação" },
            new() { Id = 2, Nome = "Aventura" },
            new() { Id = 3, Nome = "Esporte" }
        };

        _generoRepositorioMock
            .Setup(r => r.ListarAsync())
            .ReturnsAsync(generosEsperados);

        // Act
        var resultado = await _servico.ListarAsync();

        // Assert
        Assert.Equal(generosEsperados, resultado);

        _generoRepositorioMock.Verify(
            r => r.ListarAsync(),
            Times.Once);
    }
}
