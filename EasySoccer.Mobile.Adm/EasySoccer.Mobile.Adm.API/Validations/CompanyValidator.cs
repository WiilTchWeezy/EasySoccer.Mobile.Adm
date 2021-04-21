using EasySoccer.Mobile.Adm.API.ApiRequest;
using FluentValidation;

namespace EasySoccer.Mobile.Adm.API.Validations
{
    public class CompanyValidator : AbstractValidator<CompanyFormInputRequest>
    {
        public CompanyValidator()
        {
            RuleFor(x => x.CompanyDocument).MinimumLength(14).WithMessage("CNPJ inválido.").NotNull().WithMessage("CNPJ inválido.").NotEmpty().WithMessage("CNPJ inválido.");
            RuleFor(x => x.CompanyName).NotNull().WithMessage("O nome da empresa é obrigatório.").NotEmpty().WithMessage("O nome da empresa é obrigatório.");
            RuleFor(x => x.UserEmail).NotNull().WithMessage("Email do usuário inválido.").EmailAddress().WithMessage("Email do usuário inválido.");
            RuleFor(x => x.UserName).NotNull().WithMessage("O nome do usuário é obrigatório.").NotEmpty().WithMessage("O nome do usuário é obrigatório.");
        }
    }
}
