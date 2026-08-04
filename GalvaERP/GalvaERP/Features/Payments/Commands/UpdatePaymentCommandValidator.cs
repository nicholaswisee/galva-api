using FluentValidation;

namespace GalvaERP.Features.Payments.Commands;

public class UpdatePaymentCommandValidator : AbstractValidator<UpdatePaymentCommand>
{
    // ponytail: STS is nvarchar(1) on Bayar per schema; enum it on the client not in the DB.
    private static readonly string[] AllowedSts = { "0", "1", "2" };

    public UpdatePaymentCommandValidator()
    {
        RuleFor(x => x.Doku)
            .NotEmpty().WithMessage("Payment document number (Doku) is required.");

        RuleFor(x => x.IfMatchRowVersion)
            .NotEmpty().WithMessage("If-Match header (base64 RowVersion) is required for concurrency control.");

        RuleFor(x => x.STS)
            .Must(s => s is null || AllowedSts.Contains(s))
            .WithMessage("Payment status (STS) must be one of '0' (Pending), '1' (Active), or '2' (Closed).");
    }
}