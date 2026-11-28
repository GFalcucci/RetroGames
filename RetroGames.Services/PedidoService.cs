using System.Transactions;
using RetroGames.Model.Constantes;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Services.Interfaces;

namespace RetroGames.Services;

public class PedidoService : IPedidoService
{
    private readonly IRepositorySet _repositorio;
    private readonly IUsuarioAtual _usuarioAtual;

    public PedidoService(
        IRepositorySet repositorio,
        IUsuarioAtual usuarioAtual)
    {
        _repositorio = repositorio;
        _usuarioAtual = usuarioAtual;
    }

    public async Task<Pedido> InserirAsync(List<ItemPedido> itens)
    {
        var idUsuario = _usuarioAtual.Id
            ?? throw new InvalidOperationException(
                "O pedido exige um usuário autenticado.");

        var idsJogos = itens
            .Select(item => item.IdJogo)
            .Distinct()
            .ToList();

        // Lê os jogos (inclusive inativos) para decidir os erros 110 e 111.
        var resultado = await _repositorio.Jogo.ListarAsync(
            null,
            null,
            idsJogos,
            true,
            1,
            idsJogos.Count);

        var jogos = resultado.Jogos;

        // 1. Todo jogo precisa existir e estar ativo (110).
        foreach (var item in itens)
        {
            var jogo = jogos.FirstOrDefault(j => j.Id == item.IdJogo);

            if (jogo == null || !jogo.Ativo)
            {
                throw new BusinessRuleException(
                    BusinessErrors.JogoIndisponivel);
            }
        }

        // 2. Todo jogo precisa ter estoque para a quantidade pedida (111).
        foreach (var item in itens)
        {
            var jogo = jogos.First(j => j.Id == item.IdJogo);

            if (jogo.Estoque < item.Quantidade)
            {
                throw EstoqueInsuficiente(jogo);
            }
        }

        // 3. O preço vem do banco, nunca do que o front enviou.
        foreach (var item in itens)
        {
            item.PrecoUnitario = jogos.First(j => j.Id == item.IdJogo).Preco;
        }

        var pedido = new Pedido
        {
            DataPedido = DateTime.UtcNow,
            IdUsuario = idUsuario,
            IdSituacaoPedido = SituacaoPedidoIds.Pendente,
            Total = itens.Sum(item => item.PrecoUnitario * item.Quantidade)
        };

        var opcoesTransacao = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = TransactionManager.DefaultTimeout
        };

        using var transacao = new TransactionScope(
            TransactionScopeOption.Required,
            opcoesTransacao,
            TransactionScopeAsyncFlowOption.Enabled);

        pedido.Id = await _repositorio.Pedido.InserirAsync(pedido);

        foreach (var item in itens)
        {
            item.IdPedido = pedido.Id;
        }

        await _repositorio.ItemPedido.InserirAsync(itens);

        foreach (var item in itens)
        {
            // Se outra compra levou o estoque entre a leitura e aqui,
            // a baixa falha: a exceção impede o Complete e tudo é desfeito.
            var baixou = await _repositorio.Jogo.BaixarEstoqueAsync(
                item.IdJogo,
                item.Quantidade);

            if (!baixou)
            {
                throw EstoqueInsuficiente(
                    jogos.First(j => j.Id == item.IdJogo));
            }
        }

        transacao.Complete();

        return pedido;
    }

    private static BusinessRuleException EstoqueInsuficiente(Jogo jogo)
    {
        return new BusinessRuleException(
            BusinessErrors.EstoqueInsuficiente with
            {
                Mensagem = $"Estoque insuficiente para {jogo.Nome}."
            });
    }
}
