using FluentValidation;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Validators;

public class InserirItemPedidoRequestValidator
    : AbstractValidator<InserirItemPedidoRequestApiModel>
{
    public InserirItemPedidoRequestValidator()
    {
        RuleFor(modelo => modelo.IdJogo)
            .GreaterThan(0)
            .WithMessage("Jogo inválido.");

        RuleFor(modelo => modelo.Quantidade)
            .InclusiveBetween(1, 10)
            .WithMessage("A quantidade deve ser de 1 a 10.");
    }
}
