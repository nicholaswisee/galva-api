using GalvaERP.Common.Exceptions;
using GalvaERP.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GalvaERP.Features.PurchaseOrders.Commands;

public class VerifyPurchaseOrderCommandHandler : IRequestHandler<VerifyPurchaseOrderCommand, Unit>
{
    private readonly AppDbContext _context;

    public VerifyPurchaseOrderCommandHandler(AppDbContext context) => _context = context;

    public async Task<Unit> Handle(VerifyPurchaseOrderCommand request, CancellationToken cancellationToken)
    {
        var po = await _context.POSems
            .FirstOrDefaultAsync(p => p.Doku == request.Doku && p.Hapus == null, cancellationToken);
        if (po is null)
        {
            throw new NotFoundException($"Purchase Order '{request.Doku}' was not found.");
        }
        if (po.StsVerify == true)
        {
            throw new DomainException($"Purchase Order '{request.Doku}' is already verified.");
        }

        _context.Entry(po).Property(e => e.RowVersion).OriginalValue = request.IfMatchRowVersion;

        po.StsVerify = true;
        po.TglVerify = DateTime.UtcNow;

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConcurrencyException(
                $"Purchase Order '{request.Doku}' was modified by another user. Please reload and retry.");
        }

        return Unit.Value;
    }
}
