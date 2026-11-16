
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Repository.Interfaces;
using RetroGames.Services.Interfaces;

namespace RetroGames.Services;

public class PlataformaService : IPlataformaService
{
    private readonly IPlataformaRepository _plataformaRepository;

    public PlataformaService(IPlataformaRepository plataformaRepository)
    {
        _plataformaRepository = plataformaRepository;
    }

    public async Task<List<Plataforma>> ListarAsync()
    {
        return await _plataformaRepository.ListarAsync();
    }

    public async Task<Plataforma> BuscarPorIdAsync(int id)
    {
        var plataforma = await _plataformaRepository.BuscarPorIdAsync(id);

        return plataforma
            ?? throw new BusinessRuleException(
                BusinessErrors.PlataformaNaoEncontrada);
    }

    public async Task<int> InserirAsync(Plataforma plataforma)
    {
        if (await _plataformaRepository.ExisteComNomeAsync(plataforma.Nome))
        {
            throw new BusinessRuleException(
                BusinessErrors.PlataformaNomeDuplicado);
        }

        try
        {
            return await _plataformaRepository.InserirAsync(plataforma);
        }
        catch (RegistroDuplicadoException)
        {
            throw new BusinessRuleException(
                BusinessErrors.PlataformaNomeDuplicado);
        }
    }

    public async Task AlterarAsync(Plataforma plataforma)
    {
        await BuscarPorIdAsync(plataforma.Id);

        if (await _plataformaRepository.ExisteComNomeAsync(
            plataforma.Nome, plataforma.Id))
        {
            throw new BusinessRuleException(
                BusinessErrors.PlataformaNomeDuplicado);
        }

        try
        {
            await _plataformaRepository.AlterarAsync(plataforma);
        }
        catch (RegistroDuplicadoException)
        {
            throw new BusinessRuleException(
                BusinessErrors.PlataformaNomeDuplicado);
        }
    }

    public async Task ExcluirAsync(int id)
    {
        await BuscarPorIdAsync(id);

        if (await _plataformaRepository.PossuiJogosAsync(id))
        {
            throw new BusinessRuleException(
                BusinessErrors.PlataformaPossuiJogos);
        }

        await _plataformaRepository.ExcluirAsync(id);
    }
}
