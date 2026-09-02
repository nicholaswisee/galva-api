using FluentValidation;

namespace GalvaERP.Features.PurchaseOrders.Commands;

public class ConfirmPurchaseOrderCommandValidator : AbstractValidator<ConfirmPurchaseOrderCommand>
{
    public ConfirmPurchaseOrderCommandValidator()
    {
        RuleFor(x => x.Doku_POSem).NotEmpty();
        RuleFor(x => x.Tgl).NotEmpty();
        RuleFor(x => x.LineItems).NotEmpty();
        RuleForEach(x => x.LineItems).ChildRules(item =>
        {
            item.RuleFor(x => x.id_sub_posem).GreaterThan(0);
            item.RuleFor(x => x.Kode_Brg).NotEmpty();
            item.RuleFor(x => x.Jumlah).GreaterThan(0);
            item.RuleFor(x => x.Harga).GreaterThanOrEqualTo(0);
        });
    }
}
