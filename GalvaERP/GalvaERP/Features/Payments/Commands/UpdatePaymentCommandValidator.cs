using FluentValidation;

namespace GalvaERP.Features.Payments.Commands;

public class UpdatePaymentCommandValidator : AbstractValidator<UpdatePaymentCommand>
{
    public UpdatePaymentCommandValidator()
    {
        RuleFor(x => x.Doku)
            .NotEmpty().WithMessage("Doku is required.");
        RuleFor(x => x.ETag)
            .NotEmpty().WithMessage("ETag is required for concurrency control.");
    }
}
