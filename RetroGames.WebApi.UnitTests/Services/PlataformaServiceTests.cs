
using Moq;
using RetroGames.Repository;
using RetroGames.Repository.Interfaces;
using RetroGames.Services;

namespace RetroGames.WebApi.UnitTests.Services;

public class PlataformaServiceTests
{
    private readonly Mock<IPlataformaRepository> _plataformaRepositoryMock = new();
    private readonly Mock<IRepositorySet> _repositorioMock = new();

    public PlataformaServiceTests()
    {
        _repositorioMock
            .Setup(repositorio => repositorio.Plataforma)
            .Returns(_plataformaRepositoryMock.Object);
    }

    private PlataformaService CriarServico()
    {
        return new PlataformaService(_repositorioMock.Object.Plataforma);
    }

    [Fact]
    public async Task BuscarPorId_IdInexistente_LancaNaoEncontrada()
    {
        // Arrange
        const int idInexistente = 999;

        _plataformaRepositoryMock
            .Setup(repositorio => repositorio.BuscarPorIdAsync(idInexistente))
            .ReturnsAsync((RetroGames.Model.Model.Plataforma?)null);

        var servico = CriarServico();

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => servico.BuscarPorIdAsync(idInexistente));

        // Assert
        Assert.Equal(
            BusinessErrors.PlataformaNaoEncontrada.Code,
            excecao.Erro.Code);
    }
}
