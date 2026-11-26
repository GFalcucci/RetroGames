using FluentValidation;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Validators;

public class AlterarJogoRequestValidator
    : AbstractValidator<AlterarJogoRequestApiModel>
{
    public AlterarJogoRequestValidator()
    {
        RuleFor(modelo => modelo.Nome)
            .NotEmpty()
            .WithMessage("Informe o nome.");

        RuleFor(modelo => modelo.AnoLancamento)
            .InclusiveBetween(1970, 2100)
            .WithMessage("Ano de lançamento inválido.");

        RuleFor(modelo => modelo.Preco)
            .GreaterThan(0)
            .WithMessage("O preço deve ser maior que zero.");

        RuleFor(modelo => modelo.Estoque)
            .GreaterThanOrEqualTo(0)
            .WithMessage("O estoque não pode ser negativo.");
    }
}