using FluentValidation;

namespace GalvaERP.Features.Invoices.Commands;

public class UpdateAPInvoiceCommandValidator : AbstractValidator<UpdateAPInvoiceCommand>
{
    public UpdateAPInvoiceCommandValidator()
    {
        RuleFor(x => x.Doku)
            .NotEmpty().WithMessage("Doku is required.");
        RuleFor(x => x.ETag)
            .NotEmpty().WithMessage("ETag is required for concurrency control.");
    }
}
