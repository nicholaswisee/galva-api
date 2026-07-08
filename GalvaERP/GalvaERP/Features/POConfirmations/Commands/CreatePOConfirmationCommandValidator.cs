using FluentValidation;

namespace GalvaERP.Features.POConfirmations.Commands;

public class CreatePOConfirmationCommandValidator : AbstractValidator<CreatePOConfirmationCommand>
{
    public CreatePOConfirmationCommandValidator()
    {
        RuleFor(x => x.Doku_PO)
            .NotEmpty().WithMessage("PO reference (Doku_PO) is required.");

        RuleFor(x => x.Tgl)
            .NotEmpty().WithMessage("Confirmation date (Tgl) is required.");

        RuleFor(x => x.LineItems)
            .NotEmpty().WithMessage("At least one line item is required.");

        RuleForEach(x => x.LineItems)
            .ChildRules(item =>
            {
                item.RuleFor(x => x.id_sub_po)
                    .GreaterThan(0).WithMessage("PO line (id_sub_po) is required on every line item.");
                item.RuleFor(x => x.Kode_Brg)
                    .NotEmpty().WithMessage("Stock code (Kode_Brg) is required on every line item.");
                item.RuleFor(x => x.Jumlah)
                    .GreaterThan(0).WithMessage("Confirmed quantity (Jumlah) must be greater than zero on every line item.");
                item.RuleFor(x => x.Harga)
                    .GreaterThanOrEqualTo(0).WithMessage("Price (Harga) must be zero or greater on every line item.");
            });
    }
}
