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
    public async Task Inserir_RepositoryLancaRegistroDuplicado_ConverteParaErroDeNegocio()
    {
        // Arrange
        var plataforma = new Plataforma
        {
            Nome = "Nintendo"
        };

        _plataformaRepositoryMock
            .Setup(repositorio => repositorio.ExisteComNomeAsync(
                plataforma.Nome, null))
            .ReturnsAsync(false);

        _plataformaRepositoryMock
            .Setup(repositorio => repositorio.InserirAsync(plataforma))
            .ThrowsAsync(new RegistroDuplicadoException("Registro duplicado"));

        var servico = CriarServico();

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => servico.InserirAsync(plataforma));

        // Assert
        Assert.Equal(
            BusinessErrors.PlataformaNomeDuplicado.Code,
            excecao.Erro.Code);
    }

    [Fact]
    public async Task Inserir_NomeDisponivel_RetornaIdGerado()
    {
        // Arrange
        const int idGerado = 42;

        var plataforma = new Plataforma
        {
            Nome = "Sega"
        };

        _plataformaRepositoryMock
            .Setup(repositorio => repositorio.ExisteComNomeAsync(
                plataforma.Nome, null))
            .ReturnsAsync(false);

        _plataformaRepositoryMock
            .Setup(repositorio => repositorio.InserirAsync(plataforma))
            .ReturnsAsync(idGerado);

        var servico = CriarServico();

        // Act
        var resultado = await servico.InserirAsync(plataforma);

        // Assert
        Assert.Equal(idGerado, resultado);

        _plataformaRepositoryMock.Verify(
            repositorio => repositorio.InserirAsync(plataforma),
            Times.Once);
    }

    [Fact]
    public async Task Alterar_MantendoProprioNome_AceitaEGrava()
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

        // A checagem de duplicidade ignora o próprio registro (passa o id),
        // então não encontra outra plataforma com o mesmo nome.
        _plataformaRepositoryMock
            .Setup(repositorio => repositorio.ExisteComNomeAsync(
                plataforma.Nome, plataforma.Id))
            .ReturnsAsync(false);

        var servico = CriarServico();

        // Act
        await servico.AlterarAsync(plataforma);

        // Assert
        _plataformaRepositoryMock.Verify(
            repositorio => repositorio.AlterarAsync(plataforma),
            Times.Once);
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