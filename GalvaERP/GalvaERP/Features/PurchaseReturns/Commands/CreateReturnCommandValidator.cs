using FluentValidation;

namespace GalvaERP.Features.PurchaseReturns.Commands;

public sealed class CreateReturnCommandValidator : AbstractValidator<CreateReturnCommand>
{
    public CreateReturnCommandValidator()
    {
        RuleFor(command => command.Tgl).NotEmpty().WithMessage("Tgl is required.");
        RuleFor(command => command.Doku_Faktur).NotEmpty().WithMessage("Doku_Faktur is required.");
        RuleFor(command => command.Kode_Dept).MaximumLength(20);
        RuleFor(command => command.Kode_Valas).NotEmpty().MaximumLength(10).WithMessage("Kode_Valas is required.");
        RuleFor(command => command.Kurs).GreaterThanOrEqualTo(0).WithMessage("Kurs must be zero or greater.");
        RuleFor(command => command.Doku_FP).MaximumLength(50);
        RuleFor(command => command.Memo).MaximumLength(255);
        RuleFor(command => command.PPn).InclusiveBetween(0, 100).WithMessage("PPn must be between 0 and 100.");
        RuleFor(command => command.Type).MaximumLength(50);
        RuleFor(command => command.TipeRetur).MaximumLength(10);
        RuleFor(command => command.LineItems).NotEmpty().WithMessage("At least one return line is required.");

        RuleForEach(command => command.LineItems).ChildRules(line =>
        {
            line.RuleFor(item => item.Doku_Faktur).NotEmpty().WithMessage("Doku_Faktur is required.");
            line.RuleFor(item => item.Doku_LPB).NotEmpty().WithMessage("Doku_LPB is required.");
            line.RuleFor(item => item.Kode_Brg).NotEmpty().WithMessage("Kode_Brg is required.");
            line.RuleFor(item => item.Kode_Gudang).MaximumLength(20);
            line.RuleFor(item => item.Jumlah).GreaterThan(0).WithMessage("Jumlah must be greater than zero.");
            line.RuleFor(item => item.Diskon).GreaterThanOrEqualTo(0).WithMessage("Diskon must be zero or greater.");
            line.RuleFor(item => item.NoUrut).GreaterThan((short)0).WithMessage("NoUrut must be greater than zero.");
        });
    }
}
