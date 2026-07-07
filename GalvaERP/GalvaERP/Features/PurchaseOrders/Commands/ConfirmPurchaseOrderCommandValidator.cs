using FluentValidation;

namespace GalvaERP.Features.PurchaseOrders.Commands;

public class ConfirmPurchaseOrderCommandValidator : AbstractValidator<ConfirmPurchaseOrderCommand>
{
    public ConfirmPurchaseOrderCommandValidator()
    {
        RuleFor(x => x.Doku)
            .NotEmpty().WithMessage("Doku is required.");

        RuleFor(x => x.LineItems)
            .NotEmpty().WithMessage("At least one line item is required.");

        RuleForEach(x => x.LineItems)
            .ChildRules(item =>
            {
                item.RuleFor(x => x.id_sub_po)
                    .GreaterThan(0).WithMessage("id_sub_po is required.");
                item.RuleFor(x => x.Kode_Brg)
                    .NotEmpty().WithMessage("Kode_Brg is required on every line item.");
                item.RuleFor(x => x.Jumlah)
                    .GreaterThan(0).WithMessage("Confirmed quantity must be greater than zero.");
                item.RuleFor(x => x.Harga)
                    .GreaterThanOrEqualTo(0).WithMessage("Harga must be zero or greater.");
            });
    }
}
