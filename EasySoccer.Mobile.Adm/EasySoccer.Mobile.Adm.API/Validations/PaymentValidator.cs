using EasySoccer.Mobile.Adm.API.ApiRequest;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasySoccer.Mobile.Adm.API.Validations
{
    public class PaymentValidator : AbstractValidator<PaymentRequest>
    {
        public PaymentValidator()
        {
            RuleFor(x => x.CardNumber).MinimumLength(16).WithMessage("Numero do cartão inválido.");
            RuleFor(x => x.SecurityCode).MinimumLength(3).WithMessage("Cod. segurança inválido.");
            RuleFor(x => x.FinancialDocument).MinimumLength(11).WithMessage("CPF de pagamento inválido.");
            RuleFor(x => x.FinancialName).NotNull().WithMessage("O nome do usuário é obrigatório.").NotEmpty().WithMessage("O nome do usuário é obrigatório.");
            RuleFor(x => x.FinancialBirthDay).NotNull().WithMessage("A data de aniversário é obrigatória.");
            RuleFor(x => x.SelectedPlan).GreaterThanOrEqualTo(0).WithMessage("É necessário selecionar um plano.");
            RuleFor(x => x.SelectedInstallments).GreaterThanOrEqualTo(0).WithMessage("É necessário selecionar uma parcela.");
        }
    }
}
