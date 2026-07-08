using FluentValidation;

namespace GalvaERP.Features.GoodsReceipts.Commands;

public class CreateGoodsReceiptCommandValidator : AbstractValidator<CreateGoodsReceiptCommand>
{
    public CreateGoodsReceiptCommandValidator()
    {
        RuleFor(x => x.Doku_PO)
            .NotEmpty().WithMessage("PO reference (Doku_PO) is required.");

        RuleFor(x => x.Doku_PCF)
            .NotEmpty().WithMessage("PO Confirmation (Doku_PCF) is required.");

        RuleFor(x => x.LineItems)
            .NotEmpty().WithMessage("At least one line item is required.");

        RuleForEach(x => x.LineItems).ChildRules(item =>
        {
            item.RuleFor(x => x.Jumlah)
                .GreaterThan(0).WithMessage("Quantity (Jumlah) must be greater than zero.");
            item.RuleFor(x => x.Kode_Brg)
                .NotEmpty().WithMessage("Stock code (Kode_Brg) is required.");
            item.RuleFor(x => x.id_sub_po_confirmation)
                .GreaterThan(0).WithMessage("PO Confirmation line (id_sub_po_confirmation) is required.");
        });
    }
}
