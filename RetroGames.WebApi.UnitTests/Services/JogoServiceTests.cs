using Moq;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Repository.Interfaces;
using RetroGames.Services;
using RetroGames.Services.Interfaces;
using Xunit;

namespace RetroGames.WebApi.UnitTests;

public class JogoServiceTests
{
    private readonly Mock<IRepositorySet> _repositorioMock = new();

    private readonly Mock<IJogoRepository> _jogoRepositorioMock = new();
    private readonly Mock<IJogoGeneroRepository> _jogoGeneroRepositorioMock = new();
    private readonly Mock<IPlataformaRepository> _plataformaRepositorioMock = new();
    private readonly Mock<IGeneroRepository> _generoRepositorioMock = new();
    private readonly Mock<ICondicaoJogoRepository> _condicaoJogoRepositorioMock = new();
    private readonly Mock<IUsuarioAtual> _usuarioAtualMock = new();

    private readonly JogoService _servico;

    public JogoServiceTests()
    {
        _repositorioMock.SetupGet(r => r.Jogo)
            .Returns(_jogoRepositorioMock.Object);

        _repositorioMock.SetupGet(r => r.JogoGenero)
            .Returns(_jogoGeneroRepositorioMock.Object);

        _repositorioMock.SetupGet(r => r.Plataforma)
            .Returns(_plataformaRepositorioMock.Object);

        _repositorioMock.SetupGet(r => r.Genero)
            .Returns(_generoRepositorioMock.Object);

        _repositorioMock.SetupGet(r => r.CondicaoJogo)
            .Returns(_condicaoJogoRepositorioMock.Object);

        _usuarioAtualMock.SetupGet(u => u.EhAdministrador)
            .Returns(false);

        _servico = new JogoService(
            _repositorioMock.Object,
            _usuarioAtualMock.Object);
    }

    [Fact]
    public async Task InserirAsync_PlataformaInexistente_LancaExcecaoENaoGravaJogo()
    {
        // Arrange
        var jogo = CriarJogoValido();

        _plataformaRepositorioMock
            .Setup(r => r.BuscarPorIdAsync(jogo.IdPlataforma))
            .ReturnsAsync((Plataforma?)null);

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => _servico.InserirAsync(jogo, [1]));

        // Assert
        Assert.Equal(BusinessErrors.PlataformaNaoEncontrada, excecao.Erro);

        _jogoRepositorioMock.Verify(
            r => r.InserirAsync(It.IsAny<Jogo>()),
            Times.Never);

        _jogoGeneroRepositorioMock.Verify(
            r => r.InserirAsync(It.IsAny<List<JogoGenero>>()),
            Times.Never);
    }

    [Fact]
    public async Task InserirAsync_CondicaoInexistente_LancaExcecaoENaoGravaJogo()
    {
        // Arrange
        var jogo = CriarJogoValido();

        _plataformaRepositorioMock
            .Setup(r => r.BuscarPorIdAsync(jogo.IdPlataforma))
            .ReturnsAsync(new Plataforma
            {
                Id = jogo.IdPlataforma,
                Nome = "PlayStation 5"
            });

        _condicaoJogoRepositorioMock
            .Setup(r => r.ListarAsync())
            .ReturnsAsync(new List<CondicaoJogo>());

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => _servico.InserirAsync(jogo, [1]));

        // Assert
        Assert.Equal(BusinessErrors.CondicaoNaoEncontrada, excecao.Erro);

        _jogoRepositorioMock.Verify(
            r => r.InserirAsync(It.IsAny<Jogo>()),
            Times.Never);

        _jogoGeneroRepositorioMock.Verify(
            r => r.InserirAsync(It.IsAny<List<JogoGenero>>()),
            Times.Never);
    }

    [Fact]
    public async Task InserirAsync_GeneroInexistente_LancaExcecaoENaoGravaJogo()
    {
        // Arrange
        var jogo = CriarJogoValido();

        _plataformaRepositorioMock
            .Setup(r => r.BuscarPorIdAsync(jogo.IdPlataforma))
            .ReturnsAsync(new Plataforma
            {
                Id = jogo.IdPlataforma,
                Nome = "PlayStation 5"
            });

        _condicaoJogoRepositorioMock
            .Setup(r => r.ListarAsync())
            .ReturnsAsync(new List<CondicaoJogo>
            {
                new()
                {
                    Id = jogo.IdCondicaoJogo,
                    Descricao = "Novo"
                }
            });

        _generoRepositorioMock
            .Setup(r => r.ListarAsync())
            .ReturnsAsync(new List<Genero>());

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => _servico.InserirAsync(jogo, [1]));

        // Assert
        Assert.Equal(BusinessErrors.GeneroNaoEncontrado, excecao.Erro);

        _jogoRepositorioMock.Verify(
            r => r.InserirAsync(It.IsAny<Jogo>()),
            Times.Never);

        _jogoGeneroRepositorioMock.Verify(
            r => r.InserirAsync(It.IsAny<List<JogoGenero>>()),
            Times.Never);
    }

    [Fact]
    public async Task ListarAsync_FiltrandoPorGenero_EnviaSomenteIdsDosJogosDoGenero()
    {
        // Arrange
        var vinculos = new List<JogoGenero>
        {
            new() { IdJogo = 10, IdGenero = 2 },
            new() { IdJogo = 20, IdGenero = 2 },
            new() { IdJogo = 30, IdGenero = 3 },
            new() { IdJogo = 10, IdGenero = 2 }
        };

        _jogoGeneroRepositorioMock
            .Setup(r => r.ListarTodosAsync())
            .ReturnsAsync(vinculos);

        _jogoRepositorioMock
            .Setup(r => r.ListarAsync(
                It.IsAny<string?>(),
                It.IsAny<int?>(),
                It.IsAny<List<int>?>(),
                It.IsAny<bool>(),
                It.IsAny<int>(),
                It.IsAny<int>()))
            .ReturnsAsync((new List<Jogo>(), 0));

        // Act
        await _servico.ListarAsync(
            nome: null,
            idPlataforma: null,
            idGenero: 2,
            incluirInativos: false,
            pagina: 1,
            tamanhoPagina: 10);

        // Assert
        _jogoGeneroRepositorioMock.Verify(
            r => r.ListarTodosAsync(),
            Times.Once);

        _jogoRepositorioMock.Verify(
            r => r.ListarAsync(
                null,
                null,
                It.Is<List<int>?>(ids =>
                    ids != null &&
                    ids.SequenceEqual(new List<int> { 10, 20 })),
                false,
                1,
                10),
            Times.Once);
    }

    [Fact]
    public async Task ListarAsync_SemGenero_NaoConsultaVinculosENaoRestringeIds()
    {
        // Arrange
        _jogoRepositorioMock
            .Setup(r => r.ListarAsync(
                It.IsAny<string?>(),
                It.IsAny<int?>(),
                It.IsAny<List<int>?>(),
                It.IsAny<bool>(),
                It.IsAny<int>(),
                It.IsAny<int>()))
            .ReturnsAsync((new List<Jogo>(), 0));

        // Act
        await _servico.ListarAsync(
            nome: null,
            idPlataforma: null,
            idGenero: null,
            incluirInativos: false,
            pagina: 1,
            tamanhoPagina: 10);

        // Assert
        _jogoGeneroRepositorioMock.Verify(
            r => r.ListarTodosAsync(),
            Times.Never);

        _jogoRepositorioMock.Verify(
            r => r.ListarAsync(
                null,
                null,
                It.Is<List<int>?>(ids => ids == null),
                false,
                1,
                10),
            Times.Once);
    }

    [Fact]
    public async Task ListarAsync_ComJogos_RetornaNomeDaPlataformaEDescricaoDaCondicao()
    {
        // Arrange
        var jogos = new List<Jogo>
        {
            new()
            {
                Id = 1,
                Nome = "Jogo A",
                IdPlataforma = 5,
                IdCondicaoJogo = 2
            },
            new()
            {
                Id = 2,
                Nome = "Jogo B",
                IdPlataforma = 5,
                IdCondicaoJogo = 2
            }
        };

        _jogoRepositorioMock
            .Setup(r => r.ListarAsync(
                null,
                null,
                null,
                false,
                1,
                10))
            .ReturnsAsync((jogos, 2));

        _plataformaRepositorioMock
            .Setup(r => r.ListarAsync())
            .ReturnsAsync(new List<Plataforma>
            {
                new()
                {
                    Id = 5,
                    Nome = "PlayStation 5"
                }
            });

        _condicaoJogoRepositorioMock
            .Setup(r => r.ListarAsync())
            .ReturnsAsync(new List<CondicaoJogo>
            {
                new()
                {
                    Id = 2,
                    Descricao = "Seminovo"
                }
            });

        _generoRepositorioMock
            .Setup(r => r.ListarAsync())
            .ReturnsAsync(new List<Genero>());

        _jogoGeneroRepositorioMock
            .Setup(r => r.ListarPorJogosAsync(
                It.Is<List<int>>(ids => ids.SequenceEqual(new List<int> { 1, 2 }))))
            .ReturnsAsync(new List<JogoGenero>());

        // Act
        var resultado = await _servico.ListarAsync(
            nome: null,
            idPlataforma: null,
            idGenero: null,
            incluirInativos: false,
            pagina: 1,
            tamanhoPagina: 10);

        // Assert
        Assert.Equal(2, resultado.Total);
        Assert.Equal(2, resultado.Jogos.Count);

        Assert.All(resultado.Jogos, detalhe =>
        {
            Assert.Equal("PlayStation 5", detalhe.NomePlataforma);
            Assert.Equal("Seminovo", detalhe.DescricaoCondicao);
        });

        _jogoGeneroRepositorioMock.Verify(
            r => r.ListarPorJogosAsync(
                It.Is<List<int>>(ids => ids.SequenceEqual(new List<int> { 1, 2 }))),
            Times.Once);
    }

    [Fact]
    public async Task BuscarPorIdAsync_JogoInexistente_LancaExcecao()
    {
        // Arrange
        _jogoRepositorioMock
            .Setup(r => r.BuscarPorIdAsync(999))
            .ReturnsAsync((Jogo?)null);

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => _servico.BuscarPorIdAsync(999));

        // Assert
        Assert.Equal(BusinessErrors.JogoNaoEncontrado, excecao.Erro);

        _jogoRepositorioMock.Verify(
            r => r.BuscarPorIdAsync(999),
            Times.Once);
    }

    [Fact]
    public async Task DesativarAsync_JogoInexistente_LancaExcecaoENaoDesativa()
    {
        // Arrange
        _jogoRepositorioMock
            .Setup(r => r.BuscarPorIdAsync(999))
            .ReturnsAsync((Jogo?)null);

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => _servico.DesativarAsync(999));

        // Assert
        Assert.Equal(BusinessErrors.JogoNaoEncontrado, excecao.Erro);

        _jogoRepositorioMock.Verify(
            r => r.DesativarAsync(It.IsAny<int>()),
            Times.Never);
    }

    private static Jogo CriarJogoValido()
    {
        return new Jogo
        {
            Nome = "  Jogo de teste  ",
            Descricao = "Descrição do jogo",
            AnoLancamento = 2024,
            Preco = 100,
            Estoque = 10,
            Ativo = true,
            IdPlataforma = 1,
            IdCondicaoJogo = 1
        };
    }
}