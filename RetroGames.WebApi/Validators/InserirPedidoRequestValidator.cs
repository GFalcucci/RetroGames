using FluentValidation;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Validators;

public class InserirPedidoRequestValidator
    : AbstractValidator<InserirPedidoRequestApiModel>
{
    public InserirPedidoRequestValidator()
    {
        RuleFor(modelo => modelo.Itens)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("O pedido precisa ter pelo menos um item.")
            .Must(itens => itens
                .Select(item => item.IdJogo)
                .Distinct()
                .Count() == itens.Count)
            .WithMessage("Um jogo só pode aparecer uma vez no pedido.");

        RuleForEach(modelo => modelo.Itens)
            .SetValidator(new InserirItemPedidoRequestValidator());
    }
}
