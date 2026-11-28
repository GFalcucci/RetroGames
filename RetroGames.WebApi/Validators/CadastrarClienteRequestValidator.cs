using FluentValidation;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Validators;

public class CadastrarClienteRequestValidator
    : AbstractValidator<CadastrarClienteRequestApiModel>
{
    public CadastrarClienteRequestValidator()
    {
        RuleFor(modelo => modelo.Nome)
            .NotEmpty()
            .WithMessage("Informe o nome.")
            .MaximumLength(120)
            .WithMessage("O nome deve ter no máximo 120 caracteres.");

        RuleFor(modelo => modelo.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Informe o e-mail.")
            .EmailAddress()
            .WithMessage("Informe um e-mail válido.")
            .MaximumLength(254)
            .WithMessage("O e-mail deve ter no máximo 254 caracteres.");

        RuleFor(modelo => modelo.Senha)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Informe a senha.")
            .MinimumLength(8)
            .WithMessage("A senha deve ter pelo menos 8 caracteres.")
            .MaximumLength(100)
            .WithMessage("A senha deve ter no máximo 100 caracteres.");
    }
}
