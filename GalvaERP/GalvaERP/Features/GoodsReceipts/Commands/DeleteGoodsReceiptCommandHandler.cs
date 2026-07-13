using GalvaERP.Common.Exceptions;
using GalvaERP.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GalvaERP.Features.GoodsReceipts.Commands;

public class DeleteGoodsReceiptCommandHandler : IRequestHandler<DeleteGoodsReceiptCommand, Unit>
{
    private readonly AppDbContext _context;

    public DeleteGoodsReceiptCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteGoodsReceiptCommand request, CancellationToken cancellationToken)
    {
        var gr = await _context.LPBs
            .FirstOrDefaultAsync(l => l.Doku == request.Doku, cancellationToken);

        if (gr is null)
        {
            throw new NotFoundException($"Goods receipt '{request.Doku}' was not found.");
        }

        if (gr.STS != "0")
        {
            throw new DomainException(
                $"Goods receipt '{request.Doku}' can only be deleted while STS is '0' (Pending).");
        }

        _context.Entry(gr).Property(l => l.RowVersion).OriginalValue = request.IfMatchRowVersion;

        var subRows = await _context.SubLPBs
            .Where(s => s.Doku == request.Doku)
            .ToListAsync(cancellationToken);

        _context.SubLPBs.RemoveRange(subRows);
        _context.LPBs.Remove(gr);

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConcurrencyException(
                $"Goods receipt '{request.Doku}' was modified by another user. Please reload and try again.");
        }

        return Unit.Value;
    }
}
