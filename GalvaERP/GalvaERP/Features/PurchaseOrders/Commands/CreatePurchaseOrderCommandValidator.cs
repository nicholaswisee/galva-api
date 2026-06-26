using FluentValidation;
using GalvaERP.Features.PurchaseOrders.DTOs;

namespace GalvaERP.Features.PurchaseOrders.Commands;

public class CreatePurchaseOrderCommandValidator : AbstractValidator<CreatePurchaseOrderCommand>
{
    public CreatePurchaseOrderCommandValidator()
    {
        RuleFor(x => x.Kode_Supplier)
            .NotEmpty().WithMessage("Kode_Supplier is required.");

        RuleFor(x => x.Kode_dept)
            .NotEmpty().WithMessage("Kode_dept is required.");

        RuleFor(x => x.LineItems)
            .NotEmpty().WithMessage("At least one line item is required.");

        RuleForEach(x => x.LineItems)
            .ChildRules(item =>
            {
                item.RuleFor(x => x.Jumlah)
                    .GreaterThan(0).WithMessage("Jumlah must be greater than zero.");
                item.RuleFor(x => x.Kode_Brg)
                    .NotEmpty().WithMessage("Kode_Brg is required on every line item.");
            });
    }
}
