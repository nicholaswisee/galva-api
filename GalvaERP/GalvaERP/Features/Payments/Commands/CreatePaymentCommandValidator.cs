using FluentValidation;

namespace GalvaERP.Features.Payments.Commands;

public class CreatePaymentCommandValidator : AbstractValidator<CreatePaymentCommand>
{
    public CreatePaymentCommandValidator()
    {
        RuleFor(x => x.Kode_Supplier)
            .NotEmpty().WithMessage("Vendor (Kode_Supplier) is required.");

        RuleFor(x => x.Tgl)
            .NotEmpty().WithMessage("Payment date (Tgl) is required.");

        RuleFor(x => x.LineItems)
            .NotEmpty().WithMessage("At least one payment line item is required.");

        RuleForEach(x => x.LineItems).ChildRules(item =>
        {
            item.RuleFor(x => x.Doku_Faktur)
                .NotEmpty().WithMessage("Each payment line must reference an AP invoice (Doku_Faktur).");
            item.RuleFor(x => x.Doku_LPB)
                .Must(s => s is null || !string.IsNullOrWhiteSpace(s))
                .WithMessage("Doku_LPB, when supplied, must be a non-empty GR document number.");
            item.RuleFor(x => x.Nilai)
                .GreaterThan(0).WithMessage("Payment line amount (Nilai) must be greater than zero.");
            item.RuleFor(x => x.TotalNilai)
                .GreaterThan(0).WithMessage("Payment line total (TotalNilai) must be greater than zero.");
        });
    }
}