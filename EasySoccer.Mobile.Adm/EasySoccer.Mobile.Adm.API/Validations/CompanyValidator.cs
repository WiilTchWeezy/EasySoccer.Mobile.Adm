using EasySoccer.Mobile.Adm.API.ApiRequest;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasySoccer.Mobile.Adm.API.Validations
{
    public class CompanyValidator : AbstractValidator<CompanyFormInputRequest>
    {
        public CompanyValidator()
        {
            RuleFor(x => x.CompanyDocument).MinimumLength(14).WithMessage("CNPJ inválido.").NotNull().WithMessage("CNPJ inválido.").NotEmpty().WithMessage("CNPJ inválido.");
            RuleFor(x => x.CardNumber).MinimumLength(16).WithMessage("Numero do cartão inválido.");
            RuleFor(x => x.SecurityCode).MinimumLength(3).WithMessage("Cod. segurança inválido.");
            RuleFor(x => x.FinancialDocument).MinimumLength(11).WithMessage("CPF de pagamento inválido.");
            RuleFor(x => x.CompanyName).NotNull().WithMessage("O nome da empresa é obrigatório.").NotEmpty().WithMessage("O nome da empresa é obrigatório.");
            RuleFor(x => x.UserEmail).NotNull().WithMessage("Email do usuário inválido.").EmailAddress().WithMessage("Email do usuário inválido.");
        }
    }
}
