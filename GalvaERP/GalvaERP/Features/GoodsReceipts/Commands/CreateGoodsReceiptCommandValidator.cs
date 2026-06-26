using FluentValidation;

namespace GalvaERP.Features.GoodsReceipts.Commands;

public class CreateGoodsReceiptCommandValidator : AbstractValidator<CreateGoodsReceiptCommand>
{
    public CreateGoodsReceiptCommandValidator()
    {
        RuleFor(x => x.Doku_PO)
            .NotEmpty().WithMessage("Doku_PO is required.");

        RuleFor(x => x.LineItems)
            .NotEmpty().WithMessage("At least one line item is required.");

        RuleForEach(x => x.LineItems).ChildRules(item =>
        {
            item.RuleFor(x => x.Jumlah)
                .GreaterThan(0).WithMessage("Jumlah must be greater than zero.");
            item.RuleFor(x => x.Kode_Brg)
                .NotEmpty().WithMessage("Kode_Brg is required.");
        });
    }
}
