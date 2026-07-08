using FluentValidation;
using GalvaERP.Features.PurchaseOrders.DTOs;

namespace GalvaERP.Features.PurchaseOrders.Commands;

public class CreatePurchaseOrderCommandValidator : AbstractValidator<CreatePurchaseOrderCommand>
{
    public CreatePurchaseOrderCommandValidator()
    {
        RuleFor(x => x.Kode_Supplier)
            .NotEmpty().WithMessage("Vendor (Kode_Supplier) is required.");

        RuleFor(x => x.Kode_dept)
            .NotEmpty().WithMessage("Department (Kode_dept) is required.");

        RuleFor(x => x.Tgl)
            .NotEmpty().WithMessage("PO date (Tgl) is required.");

        RuleFor(x => x.Kode_Valas)
            .NotEmpty().WithMessage("Currency (Kode_Valas) is required.");

        RuleFor(x => x.LineItems)
            .NotEmpty().WithMessage("At least one line item is required.");

        RuleForEach(x => x.LineItems)
            .ChildRules(item =>
            {
                item.RuleFor(x => x.Kode_Brg)
                    .NotEmpty().WithMessage("Stock code (Kode_Brg) is required on every line item.");
                item.RuleFor(x => x.Jumlah)
                    .GreaterThan(0).WithMessage("Quantity (Jumlah) must be greater than zero on every line item.");
                item.RuleFor(x => x.Harga)
                    .GreaterThanOrEqualTo(0).WithMessage("Price (Harga) must be zero or greater on every line item.");
            });
    }
}
