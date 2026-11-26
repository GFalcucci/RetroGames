
using System.Transactions;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Services.Interfaces;
using RetroGames.Services.ServiceModels;

namespace RetroGames.Services;

public class JogoService : IJogoService
{
    private readonly IRepositorySet _repositorio;
    private readonly IUsuarioAtual _usuarioAtual;

    public JogoService(
        IRepositorySet repositorio,
        IUsuarioAtual usuarioAtual)
    {
        _repositorio = repositorio;
        _usuarioAtual = usuarioAtual;
    }

    public async Task<(List<JogoDetalheServiceModel> Jogos, int Total)> ListarAsync(
        string? nome,
        int? idPlataforma,
        int? idGenero,
        bool incluirInativos,
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

        // Somente administradores podem solicitar jogos inativos.
        var podeIncluirInativos =
            incluirInativos && _usuarioAtual.EhAdministrador;

        var resultado = await _repositorio.Jogo.ListarAsync(
            nome,
            idPlataforma,
            idsJogos,
            podeIncluirInativos,
            pagina,
            tamanhoPagina);

        var detalhes = await MontarDetalhesAsync(resultado.Jogos);

        return (detalhes, resultado.Total);
    }

    public async Task<JogoDetalheServiceModel> BuscarPorIdAsync(int id)
    {
        var jogo = await _repositorio.Jogo.BuscarPorIdAsync(id);

        // Jogos inativos não são visíveis para usuários comuns.
        if (jogo == null ||
            (!jogo.Ativo && !_usuarioAtual.EhAdministrador))
        {
            throw new BusinessRuleException(BusinessErrors.JogoNaoEncontrado);
        }

        var detalhes = await MontarDetalhesAsync([jogo]);

        return detalhes[0];
    }

    public async Task<int> InserirAsync(
        Jogo jogo,
        List<int> idsGeneros)
    {
        jogo.Nome = jogo.Nome.Trim();
        jogo.Ativo = true;

        await ValidarReferenciasAsync(jogo, idsGeneros);

        var opcoesTransacao = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = TransactionManager.DefaultTimeout
        };

        using var transacao = new TransactionScope(
            TransactionScopeOption.Required,
            opcoesTransacao,
            TransactionScopeAsyncFlowOption.Enabled);

        var idJogo = await _repositorio.Jogo.InserirAsync(jogo);

        var vinculos = idsGeneros
            .Distinct()
            .Select(idGenero => new JogoGenero
            {
                IdJogo = idJogo,
                IdGenero = idGenero
            })
            .ToList();

        await _repositorio.JogoGenero.InserirAsync(vinculos);

        transacao.Complete();

        return idJogo;
    }

    public async Task AlterarAsync(
        Jogo jogo,
        List<int> idsGeneros)
    {
        var jogoExistente = await _repositorio.Jogo.BuscarPorIdAsync(
            jogo.Id);

        if (jogoExistente == null)
        {
            throw new BusinessRuleException(BusinessErrors.JogoNaoEncontrado);
        }

        jogo.Nome = jogo.Nome.Trim();

        // Preserva os dados controlados pelo sistema.
        jogo.Ativo = jogoExistente.Ativo;
        jogo.DataCadastro = jogoExistente.DataCadastro;

        await ValidarReferenciasAsync(jogo, idsGeneros);

        var opcoesTransacao = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = TransactionManager.DefaultTimeout
        };

        using var transacao = new TransactionScope(
            TransactionScopeOption.Required,
            opcoesTransacao,
            TransactionScopeAsyncFlowOption.Enabled);

        await _repositorio.Jogo.AlterarAsync(jogo);

        await _repositorio.JogoGenero.RemoverPorJogoAsync(jogo.Id);

        var vinculos = idsGeneros
            .Distinct()
            .Select(idGenero => new JogoGenero
            {
                IdJogo = jogo.Id,
                IdGenero = idGenero
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
        Jogo jogo,
        List<int> idsGeneros)
    {
        var plataforma = await _repositorio.Plataforma.BuscarPorIdAsync(
            jogo.IdPlataforma);

        if (plataforma == null)
        {
            throw new BusinessRuleException(
                BusinessErrors.PlataformaNaoEncontrada);
        }

        var condicoes = await _repositorio.CondicaoJogo.ListarAsync();

        if (!condicoes.Any(condicao =>
            condicao.Id == jogo.IdCondicaoJogo))
        {
            throw new BusinessRuleException(
                BusinessErrors.CondicaoNaoEncontrada);
        }

        var generos = await _repositorio.Genero.ListarAsync();

        var idsUnicos = idsGeneros
            .Distinct()
            .ToList();

        if (idsUnicos.Count == 0 ||
            idsUnicos.Any(idGenero =>
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
