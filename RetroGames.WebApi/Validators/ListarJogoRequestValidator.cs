
using FluentValidation;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Validators;

public class ListarJogoRequestValidator : AbstractValidator<ListarJogoRequestApiModel>
{
    public ListarJogoRequestValidator()
    {
        RuleFor(request => request.Pagina)
            .GreaterThan(0)
            .WithMessage("A página deve ser maior que zero.");
    }
}
