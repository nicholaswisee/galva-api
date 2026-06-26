using FluentValidation;
using GalvaERP.Features.PurchaseRequisitions.DTOs;

namespace GalvaERP.Features.PurchaseRequisitions.Commands;

public class CreatePurchaseRequisitionCommandValidator : AbstractValidator<CreatePurchaseRequisitionCommand>
{
    public CreatePurchaseRequisitionCommandValidator()
    {
        RuleFor(x => x.Kode_Dept)
            .NotEmpty().WithMessage("Kode_Dept is required.");

        RuleFor(x => x.LineItems)
            .NotEmpty().WithMessage("At least one line item is required.");

        RuleForEach(x => x.LineItems)
            .ChildRules(item =>
            {
                item.RuleFor(x => x.Jumlah)
                    .GreaterThan(0).WithMessage("Jumlah must be greater than zero.");
            });

        RuleForEach(x => x.LineItems)
            .ChildRules(item =>
            {
                item.RuleFor(x => x.Kode_Brg)
                    .NotEmpty().WithMessage("Kode_Brg is required on every line item.");
            });
    }
}
