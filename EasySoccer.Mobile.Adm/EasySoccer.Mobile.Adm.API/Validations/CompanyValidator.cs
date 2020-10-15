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
            RuleFor(x => x.CompanyName).NotNull().WithMessage("O nome da empresa é obrigatório.").NotEmpty().WithMessage("O nome da empresa é obrigatório."); ;
            RuleFor(x => x.UserEmail).NotNull().WithMessage("Email do usuário inválido.").EmailAddress().WithMessage("Email do usuário inválido.");
        }
    }
}
