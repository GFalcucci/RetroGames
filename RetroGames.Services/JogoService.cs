
using System.Transactions;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Services.Interfaces;
using RetroGames.Services.ServiceModels;

namespace RetroGames.Services;

public class JogoService : IJogoService
{
    private readonly IRepositorySet _repositorio;

    public JogoService(IRepositorySet repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<(List<JogoDetalheServiceModel> Jogos, int Total)> ListarAsync(
        string? nome,
        int? idPlataforma,
        int? idGenero,
        int pagina,
        int tamanhoPagina)
    {
        List<int>? idsJogos = null;

        if (idGenero.HasValue)
        {
            var vinculos = await _repositorio.JogoGenero.ListarTodosAsync();

            idsJogos = vinculos
                .Where(vinculo => vinculo.IdGenero == idGenero.Value)
                .Select(vinculo => vinculo.IdJogo)
                .Distinct()
                .ToList();

            if (idsJogos.Count == 0)
            {
                return ([], 0);
            }
        }

        var resultado = await _repositorio.Jogo.ListarAsync(
            nome,
            idPlataforma,
            idsJogos,
            false,
            pagina,
            tamanhoPagina);

        var detalhes = await MontarDetalhesAsync(resultado.Jogos);

        return (detalhes, resultado.Total);
    }

    public async Task<JogoDetalheServiceModel> BuscarPorIdAsync(int id)
    {
        var jogo = await _repositorio.Jogo.BuscarPorIdAsync(id);

        if (jogo == null)
        {
            throw new BusinessRuleException(BusinessErrors.JogoNaoEncontrado);
        }

        var detalhes = await MontarDetalhesAsync([jogo]);

        return detalhes[0];
    }

    public async Task<int> InserirAsync(JogoDetalheServiceModel detalhe)
    {
        detalhe.Jogo.Nome = detalhe.Jogo.Nome.Trim();
        detalhe.Jogo.Ativo = true;

        await ValidarReferenciasAsync(detalhe);

        var opcoesTransacao = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = TransactionManager.DefaultTimeout
        };

        using var transacao = new TransactionScope(
            TransactionScopeOption.Required,
            opcoesTransacao,
            TransactionScopeAsyncFlowOption.Enabled);

        var idJogo = await _repositorio.Jogo.InserirAsync(detalhe.Jogo);

        var vinculos = detalhe.Generos
            .Select(genero => new JogoGenero
            {
                IdJogo = idJogo,
                IdGenero = genero.Id
            })
            .ToList();

        await _repositorio.JogoGenero.InserirAsync(vinculos);

        transacao.Complete();

        return idJogo;
    }

    public async Task AlterarAsync(JogoDetalheServiceModel detalhe)
    {
        var jogoExistente = await _repositorio.Jogo.BuscarPorIdAsync(
            detalhe.Jogo.Id);

        if (jogoExistente == null)
        {
            throw new BusinessRuleException(BusinessErrors.JogoNaoEncontrado);
        }

        detalhe.Jogo.Nome = detalhe.Jogo.Nome.Trim();

        await ValidarReferenciasAsync(detalhe);

        var opcoesTransacao = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = TransactionManager.DefaultTimeout
        };

        using var transacao = new TransactionScope(
            TransactionScopeOption.Required,
            opcoesTransacao,
            TransactionScopeAsyncFlowOption.Enabled);

        await _repositorio.Jogo.AlterarAsync(detalhe.Jogo);

        await _repositorio.JogoGenero.RemoverPorJogoAsync(detalhe.Jogo.Id);

        var vinculos = detalhe.Generos
            .Select(genero => new JogoGenero
            {
                IdJogo = detalhe.Jogo.Id,
                IdGenero = genero.Id
            })
            .ToList();

        await _repositorio.JogoGenero.InserirAsync(vinculos);

        transacao.Complete();
    }

    public async Task DesativarAsync(int id)
    {
        var jogo = await _repositorio.Jogo.BuscarPorIdAsync(id);

        if (jogo == null)
        {
            throw new BusinessRuleException(BusinessErrors.JogoNaoEncontrado);
        }

        await _repositorio.Jogo.DesativarAsync(id);
    }

    private async Task ValidarReferenciasAsync(
        JogoDetalheServiceModel detalhe)
    {
        var plataforma = await _repositorio.Plataforma.BuscarPorIdAsync(
            detalhe.Jogo.IdPlataforma);

        if (plataforma == null)
        {
            throw new BusinessRuleException(
                BusinessErrors.PlataformaNaoEncontrada);
        }

        var condicoes = await _repositorio.CondicaoJogo.ListarAsync();

        if (!condicoes.Any(condicao =>
            condicao.Id == detalhe.Jogo.IdCondicaoJogo))
        {
            throw new BusinessRuleException(
                BusinessErrors.CondicaoNaoEncontrada);
        }

        var generos = await _repositorio.Genero.ListarAsync();

        var idsGeneros = detalhe.Generos
            .Select(genero => genero.Id)
            .Distinct()
            .ToList();

        if (idsGeneros.Count == 0 ||
            idsGeneros.Any(idGenero =>
                !generos.Any(genero => genero.Id == idGenero)))
        {
            throw new BusinessRuleException(
                BusinessErrors.GeneroNaoEncontrado);
        }
    }

    private async Task<List<JogoDetalheServiceModel>> MontarDetalhesAsync(
        List<Jogo> jogos)
    {
        if (jogos.Count == 0)
        {
            return [];
        }

        var idsJogos = jogos.Select(jogo => jogo.Id).ToList();

        var plataformas = await _repositorio.Plataforma.ListarAsync();
        var condicoes = await _repositorio.CondicaoJogo.ListarAsync();
        var generos = await _repositorio.Genero.ListarAsync();
        var vinculos = await _repositorio.JogoGenero.ListarPorJogosAsync(
            idsJogos);

        var detalhes = jogos.Select(jogo =>
        {
            var plataforma = plataformas.FirstOrDefault(
                item => item.Id == jogo.IdPlataforma);

            var condicao = condicoes.FirstOrDefault(
                item => item.Id == jogo.IdCondicaoJogo);

            var generosDoJogo = vinculos
                .Where(vinculo => vinculo.IdJogo == jogo.Id)
                .Select(vinculo => generos.FirstOrDefault(
                    genero => genero.Id == vinculo.IdGenero))
                .Where(genero => genero != null)
                .Select(genero => genero!)
                .ToList();

            return new JogoDetalheServiceModel
            {
                Jogo = jogo,
                NomePlataforma = plataforma?.Nome ?? string.Empty,
                DescricaoCondicao = condicao?.Descricao ?? string.Empty,
                Generos = generosDoJogo
            };
        }).ToList();

        return detalhes;
    }
}
