using FluentValidation;

namespace GalvaERP.Features.PurchaseReturns.Commands;

public sealed class UpdateReturnCommandValidator : AbstractValidator<UpdateReturnCommand>
{
    public UpdateReturnCommandValidator()
    {
        RuleFor(command => command.Doku).NotEmpty().WithMessage("Doku is required.");
        RuleFor(command => command.STS).NotEmpty().MaximumLength(2).WithMessage("STS is required.");
        RuleFor(command => command.Memo).MaximumLength(255);
        RuleFor(command => command.StatusGL).MaximumLength(10);
    }
}
