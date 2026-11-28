using Moq;
using RetroGames.Model.Constantes;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Repository.Interfaces;
using RetroGames.Services;
using RetroGames.Services.Interfaces;

namespace RetroGames.WebApi.UnitTests.Services;

public class PedidoServiceTests
{
    private const int IdUsuarioLogado = 7;

    private readonly Mock<IRepositorySet> _repositorioMock = new();
    private readonly Mock<IJogoRepository> _jogoRepositorioMock = new();
    private readonly Mock<IPedidoRepository> _pedidoRepositorioMock = new();
    private readonly Mock<IItemPedidoRepository> _itemPedidoRepositorioMock = new();
    private readonly Mock<IUsuarioAtual> _usuarioAtualMock = new();

    private readonly PedidoService _servico;

    public PedidoServiceTests()
    {
        _repositorioMock.Setup(r => r.Jogo).Returns(_jogoRepositorioMock.Object);
        _repositorioMock.Setup(r => r.Pedido).Returns(_pedidoRepositorioMock.Object);
        _repositorioMock.Setup(r => r.ItemPedido).Returns(_itemPedidoRepositorioMock.Object);

        _usuarioAtualMock.Setup(u => u.Id).Returns(IdUsuarioLogado);

        _servico = new PedidoService(
            _repositorioMock.Object,
            _usuarioAtualMock.Object);
    }

    private void ConfigurarJogos(params Jogo[] jogos)
    {
        var lista = jogos.ToList();

        _jogoRepositorioMock
            .Setup(r => r.ListarAsync(
                null,
                null,
                It.IsAny<List<int>?>(),
                true,
                1,
                It.IsAny<int>()))
            .ReturnsAsync((lista, lista.Count));
    }

    private static Jogo CriarJogo(
        int id,
        decimal preco,
        int estoque,
        bool ativo = true)
    {
        return new Jogo
        {
            Id = id,
            Nome = $"Jogo {id}",
            Preco = preco,
            Estoque = estoque,
            Ativo = ativo
        };
    }

    [Fact]
    public async Task Inserir_ComprasValidas_GravaPedidoItensEBaixaEstoque()
    {
        // Arrange
        ConfigurarJogos(
            CriarJogo(1, 100m, 5),
            CriarJogo(2, 49.90m, 3));

        _pedidoRepositorioMock
            .Setup(r => r.InserirAsync(It.IsAny<Pedido>()))
            .ReturnsAsync(10);

        _jogoRepositorioMock
            .Setup(r => r.BaixarEstoqueAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(true);

        // O front tenta mandar um preço falso: ele deve ser ignorado.
        var itens = new List<ItemPedido>
        {
            new() { IdJogo = 1, Quantidade = 2, PrecoUnitario = 0.01m },
            new() { IdJogo = 2, Quantidade = 1, PrecoUnitario = 0.01m }
        };

        // Act
        var pedido = await _servico.InserirAsync(itens);

        // Assert
        Assert.Equal(10, pedido.Id);
        Assert.Equal(IdUsuarioLogado, pedido.IdUsuario);
        Assert.Equal(SituacaoPedidoIds.Pendente, pedido.IdSituacaoPedido);
        Assert.Equal(249.90m, pedido.Total);
        Assert.Equal(DateTimeKind.Utc, pedido.DataPedido.Kind);

        _itemPedidoRepositorioMock.Verify(
            r => r.InserirAsync(It.Is<List<ItemPedido>>(lista =>
                lista.Count == 2 &&
                lista.All(item => item.IdPedido == 10) &&
                lista.Single(item => item.IdJogo == 1).PrecoUnitario == 100m &&
                lista.Single(item => item.IdJogo == 2).PrecoUnitario == 49.90m)),
            Times.Once);

        _jogoRepositorioMock.Verify(
            r => r.BaixarEstoqueAsync(1, 2), Times.Once);
        _jogoRepositorioMock.Verify(
            r => r.BaixarEstoqueAsync(2, 1), Times.Once);
    }

    [Fact]
    public async Task Inserir_JogoInexistente_LancaJogoIndisponivelENaoGrava()
    {
        // Arrange: o banco devolve só o jogo 1; o jogo 99 não existe.
        ConfigurarJogos(CriarJogo(1, 100m, 5));

        var itens = new List<ItemPedido>
        {
            new() { IdJogo = 1, Quantidade = 1 },
            new() { IdJogo = 99, Quantidade = 1 }
        };

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => _servico.InserirAsync(itens));

        // Assert
        Assert.Equal(BusinessErrors.JogoIndisponivel.Code, excecao.Erro.Code);
        VerificarQueNadaFoiGravado();
    }

    [Fact]
    public async Task Inserir_JogoInativo_LancaJogoIndisponivelENaoGrava()
    {
        // Arrange
        ConfigurarJogos(CriarJogo(1, 100m, 5, ativo: false));

        var itens = new List<ItemPedido>
        {
            new() { IdJogo = 1, Quantidade = 1 }
        };

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => _servico.InserirAsync(itens));

        // Assert
        Assert.Equal(BusinessErrors.JogoIndisponivel.Code, excecao.Erro.Code);
        VerificarQueNadaFoiGravado();
    }

    [Fact]
    public async Task Inserir_EstoqueInsuficiente_LancaErroComNomeDoJogoENaoGrava()
    {
        // Arrange: há 1 em estoque e o cliente quer 2.
        ConfigurarJogos(CriarJogo(1, 100m, 1));

        var itens = new List<ItemPedido>
        {
            new() { IdJogo = 1, Quantidade = 2 }
        };

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => _servico.InserirAsync(itens));

        // Assert
        Assert.Equal(BusinessErrors.EstoqueInsuficiente.Code, excecao.Erro.Code);
        Assert.Equal(
            BusinessErrors.EstoqueInsuficiente.Status,
            excecao.Erro.Status);
        Assert.Contains("Jogo 1", excecao.Message);
        VerificarQueNadaFoiGravado();
    }

    [Fact]
    public async Task Inserir_BaixaDeEstoqueFalha_LancaEstoqueInsuficiente()
    {
        // Arrange: a leitura mostrava estoque, mas outra compra o levou
        // antes do UPDATE (o repositório devolve false).
        ConfigurarJogos(CriarJogo(1, 100m, 5));

        _pedidoRepositorioMock
            .Setup(r => r.InserirAsync(It.IsAny<Pedido>()))
            .ReturnsAsync(10);

        _jogoRepositorioMock
            .Setup(r => r.BaixarEstoqueAsync(1, 2))
            .ReturnsAsync(false);

        var itens = new List<ItemPedido>
        {
            new() { IdJogo = 1, Quantidade = 2 }
        };

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => _servico.InserirAsync(itens));

        // Assert: a exceção sai do TransactionScope sem Complete(),
        // o que desfaz pedido, itens e baixas já feitas.
        Assert.Equal(BusinessErrors.EstoqueInsuficiente.Code, excecao.Erro.Code);
    }

    private void VerificarQueNadaFoiGravado()
    {
        _pedidoRepositorioMock.Verify(
            r => r.InserirAsync(It.IsAny<Pedido>()),
            Times.Never);

        _itemPedidoRepositorioMock.Verify(
            r => r.InserirAsync(It.IsAny<List<ItemPedido>>()),
            Times.Never);

        _jogoRepositorioMock.Verify(
            r => r.BaixarEstoqueAsync(It.IsAny<int>(), It.IsAny<int>()),
            Times.Never);
    }
}
