
using Moq;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Repository.Interfaces;
using RetroGames.Services;

namespace RetroGames.WebApi.UnitTests.Services;

public class PlataformaServiceTests
{
    private readonly Mock<IPlataformaRepository> _plataformaRepositoryMock = new();

    private PlataformaService CriarServico()
    {
        return new PlataformaService(_plataformaRepositoryMock.Object);
    }

    [Fact]
    public async Task BuscarPorId_IdInexistente_LancaNaoEncontrada()
    {
        // Arrange
        const int idInexistente = 999;

        _plataformaRepositoryMock
            .Setup(repositorio => repositorio.BuscarPorIdAsync(idInexistente))
            .ReturnsAsync((Plataforma?)null);

        var servico = CriarServico();

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => servico.BuscarPorIdAsync(idInexistente));

        // Assert
        Assert.Equal(
            BusinessErrors.PlataformaNaoEncontrada.Code,
            excecao.Erro.Code);
    }

    [Fact]
    public async Task Inserir_NomeDuplicado_LancaErroENaoInsere()
    {
        // Arrange
        var plataforma = new Plataforma
        {
            Nome = "Nintendo"
        };

        _plataformaRepositoryMock
            .Setup(repositorio => repositorio.ExisteComNomeAsync(
                plataforma.Nome, null))
            .ReturnsAsync(true);

        var servico = CriarServico();

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => servico.InserirAsync(plataforma));

        // Assert
        Assert.Equal(
            BusinessErrors.PlataformaNomeDuplicado.Code,
            excecao.Erro.Code);

        _plataformaRepositoryMock.Verify(
            repositorio => repositorio.InserirAsync(
                It.IsAny<Plataforma>()),
            Times.Never);
    }

    [Fact]
    public async Task Alterar_NomeDuplicado_LancaErroENaoAltera()
    {
        // Arrange
        var plataforma = new Plataforma
        {
            Id = 1,
            Nome = "Nintendo"
        };

        _plataformaRepositoryMock
            .Setup(repositorio => repositorio.BuscarPorIdAsync(
                plataforma.Id))
            .ReturnsAsync(plataforma);

        _plataformaRepositoryMock
            .Setup(repositorio => repositorio.ExisteComNomeAsync(
                plataforma.Nome, plataforma.Id))
            .ReturnsAsync(true);

        var servico = CriarServico();

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => servico.AlterarAsync(plataforma));

        // Assert
        Assert.Equal(
            BusinessErrors.PlataformaNomeDuplicado.Code,
            excecao.Erro.Code);

        _plataformaRepositoryMock.Verify(
            repositorio => repositorio.AlterarAsync(
                It.IsAny<Plataforma>()),
            Times.Never);
    }

    [Fact]
    public async Task Excluir_PlataformaComJogos_LancaErroENaoExclui()
    {
        // Arrange
        const int idPlataforma = 1;

        _plataformaRepositoryMock
            .Setup(repositorio => repositorio.BuscarPorIdAsync(
                idPlataforma))
            .ReturnsAsync(new Plataforma
            {
                Id = idPlataforma,
                Nome = "Nintendo"
            });

        _plataformaRepositoryMock
            .Setup(repositorio => repositorio.PossuiJogosAsync(
                idPlataforma))
            .ReturnsAsync(true);

        var servico = CriarServico();

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => servico.ExcluirAsync(idPlataforma));

        // Assert
        Assert.Equal(
            BusinessErrors.PlataformaPossuiJogos.Code,
            excecao.Erro.Code);

        _plataformaRepositoryMock.Verify(
            repositorio => repositorio.ExcluirAsync(idPlataforma),
            Times.Never);
    }
}
