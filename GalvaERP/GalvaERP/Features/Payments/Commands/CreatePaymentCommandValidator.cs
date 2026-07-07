using FluentValidation;

namespace GalvaERP.Features.Payments.Commands;

public class CreatePaymentCommandValidator : AbstractValidator<CreatePaymentCommand>
{
    public CreatePaymentCommandValidator()
    {
        RuleFor(x => x.Kode_Supplier)
            .NotEmpty().WithMessage("Kode_Supplier is required.");

        RuleFor(x => x.LineItems)
            .NotEmpty().WithMessage("At least one line item is required.");

        RuleForEach(x => x.LineItems).ChildRules(item =>
        {
            item.RuleFor(x => x.Doku_LPB)
                .NotEmpty().WithMessage("Doku_LPB is required.");
            item.RuleFor(x => x.TotalNilai)
                .GreaterThan(0).WithMessage("TotalNilai must be greater than zero.");
        });
    }
}
