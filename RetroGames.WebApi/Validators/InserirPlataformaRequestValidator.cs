using FluentValidation;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Validators;

public class InserirPlataformaRequestValidator
    : AbstractValidator<InserirPlataformaRequestApiModel>
{
    public InserirPlataformaRequestValidator()
    {
        RuleFor(modelo => modelo.Nome)
            .NotEmpty()
            .WithMessage("Informe o nome.");

        RuleFor(modelo => modelo.Fabricante)
            .NotEmpty()
            .WithMessage("Informe o fabricante.");

        RuleFor(modelo => modelo.AnoLancamento)
            .InclusiveBetween(1970, 2100)
            .WithMessage("Ano de lançamento inválido.");
    }
}