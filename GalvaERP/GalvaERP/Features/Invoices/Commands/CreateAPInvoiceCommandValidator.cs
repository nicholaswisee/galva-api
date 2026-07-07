using FluentValidation;

namespace GalvaERP.Features.Invoices.Commands;

public class CreateAPInvoiceCommandValidator : AbstractValidator<CreateAPInvoiceCommand>
{
    public CreateAPInvoiceCommandValidator()
    {
        RuleFor(x => x.Kode_Supplier)
            .NotEmpty().WithMessage("Kode_Supplier is required.");

        RuleFor(x => x.GRLinks)
            .NotEmpty().WithMessage("At least one GR link is required.");

        RuleFor(x => x.Nilai)
            .GreaterThan(0).WithMessage("Nilai must be greater than zero.");

        RuleForEach(x => x.GRLinks).ChildRules(item =>
        {
            item.RuleFor(x => x.Doku_LPB)
                .NotEmpty().WithMessage("Doku_LPB is required.");
            item.RuleFor(x => x.NilaiLPB)
                .GreaterThan(0).WithMessage("NilaiLPB must be greater than zero.");
        });
    }
}