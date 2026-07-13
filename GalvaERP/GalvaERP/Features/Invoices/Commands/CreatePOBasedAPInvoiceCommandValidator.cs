using FluentValidation;

namespace GalvaERP.Features.Invoices.Commands;

public class CreatePOBasedAPInvoiceCommandValidator : AbstractValidator<CreatePOBasedAPInvoiceCommand>
{
    public CreatePOBasedAPInvoiceCommandValidator()
    {
        RuleFor(x => x.Kode_Supplier)
            .NotEmpty().WithMessage("Vendor (Kode_Supplier) is required.");

        RuleFor(x => x.Tgl)
            .NotEmpty().WithMessage("Date (Tgl) is required.");

        RuleFor(x => x.POLinks)
            .NotEmpty().WithMessage("At least one PO link is required.");

        RuleFor(x => x.Amount)
            .GreaterThanOrEqualTo(0).WithMessage("Amount must be zero or greater.");

        RuleForEach(x => x.POLinks).ChildRules(item =>
        {
            item.RuleFor(x => x.Doku_PO)
                .NotEmpty().WithMessage("PO number is required.");
            item.RuleFor(x => x.Amount)
                .GreaterThanOrEqualTo(0).WithMessage("PO amount must be zero or greater.");
        });

        RuleForEach(x => x.CostLines).ChildRules(item =>
        {
            item.RuleFor(x => x.Amount)
                .GreaterThanOrEqualTo(0).WithMessage("Cost line amount must be zero or greater.");
        });
    }
}
