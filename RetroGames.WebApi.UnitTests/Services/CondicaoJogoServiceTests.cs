
using Moq;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Repository.Interfaces;
using RetroGames.Services;
using Xunit;

namespace RetroGames.WebApi.UnitTests;

public class CondicaoJogoServiceTests
{
    private readonly Mock<IRepositorySet> _repositorioMock = new();
    private readonly Mock<ICondicaoJogoRepository> _condicaoRepositorioMock = new();

    private readonly CondicaoJogoService _servico;

    public CondicaoJogoServiceTests()
    {
        _repositorioMock
            .SetupGet(r => r.CondicaoJogo)
            .Returns(_condicaoRepositorioMock.Object);

        _servico = new CondicaoJogoService(_repositorioMock.Object);
    }

    [Fact]
    public async Task ListarAsync_DeveRetornarCondicoesDoRepositorio()
    {
        // Arrange
        var condicoesEsperadas = new List<CondicaoJogo>
        {
            new() { Id = 1, Descricao = "Novo" },
            new() { Id = 2, Descricao = "Seminovo" },
            new() { Id = 3, Descricao = "Usado" }
        };

        _condicaoRepositorioMock
            .Setup(r => r.ListarAsync())
            .ReturnsAsync(condicoesEsperadas);

        // Act
        var resultado = await _servico.ListarAsync();

        // Assert
        Assert.Equal(condicoesEsperadas, resultado);

        _condicaoRepositorioMock.Verify(
            r => r.ListarAsync(),
            Times.Once);
    }
}
