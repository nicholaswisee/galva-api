using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GalvaERP.Common.Exceptions;
using GalvaERP.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GalvaERP.Features.PurchaseOrders.Commands;

public class DeletePurchaseOrderCommandHandler : IRequestHandler<DeletePurchaseOrderCommand, Unit>
{
    private readonly AppDbContext _context;

    public DeletePurchaseOrderCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeletePurchaseOrderCommand request, CancellationToken cancellationToken)
    {
        var po = await _context.POs
            .FirstOrDefaultAsync(p => p.Doku == request.Doku, cancellationToken);

        if (po is null || po.Hapus == "Y")
        {
            throw new NotFoundException($"Purchase Order '{request.Doku}' was not found.");
        }

        if (!po.RowVersion.SequenceEqual(request.IfMatchRowVersion))
        {
            throw new ConcurrencyException(
                $"Purchase Order '{request.Doku}' was modified by another user. Please reload and try again.");
        }

        if (po.STS != "0")
        {
            throw new DomainException(
                $"Purchase Order '{request.Doku}' can only be deleted while STS is '0' (Pending).");
        }

        po.Hapus = "Y";
        _context.Entry(po).Property(p => p.RowVersion).OriginalValue = request.IfMatchRowVersion;

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConcurrencyException(
                $"Purchase Order '{request.Doku}' was modified by another user. Please reload and try again.");
        }

        return Unit.Value;
    }
}
