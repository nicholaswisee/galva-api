using FluentValidation;

namespace GalvaERP.Features.GoodsReceipts.Commands;

public class UpdateGoodsReceiptCommandValidator : AbstractValidator<UpdateGoodsReceiptCommand>
{
    public UpdateGoodsReceiptCommandValidator()
    {
        RuleFor(x => x.Doku)
            .NotEmpty().WithMessage("Doku is required.");
        RuleFor(x => x.ETag)
            .NotEmpty().WithMessage("ETag is required for concurrency control.");
    }
}
