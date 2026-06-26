using GalvaERP.Common.Exceptions;
using GalvaERP.Features.GoodsReceipts.DTOs;
using GalvaERP.Features.GoodsReceipts.Queries;
using GalvaERP.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GalvaERP.Features.GoodsReceipts.Commands;

public class UpdateGoodsReceiptCommandHandler : IRequestHandler<UpdateGoodsReceiptCommand, GRDetailDto>
{
    private readonly AppDbContext _context;

    public UpdateGoodsReceiptCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<GRDetailDto> Handle(UpdateGoodsReceiptCommand request, CancellationToken cancellationToken)
    {
        var lpb = await _context.LPBs
            .FirstOrDefaultAsync(l => l.Doku == request.Doku, cancellationToken);

        if (lpb is null)
        {
            throw new NotFoundException($"Goods receipt '{request.Doku}' not found");
        }

        // Optimistic concurrency: check ETag matches the current RowVersion.
        var currentETag = Convert.ToBase64String(lpb.RowVersion);
        if (!string.Equals(currentETag, request.ETag, StringComparison.Ordinal))
        {
            throw new ConcurrencyException(
                $"Goods receipt '{request.Doku}' was modified by another user. Please reload and retry.");
        }

        if (request.STS is not null) lpb.STS = request.STS;
        if (request.Status is not null) lpb.Status = request.Status;
        if (request.Memo is not null) lpb.Memo = request.Memo;
        if (request.PPN.HasValue) lpb.PPN = request.PPN;

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConcurrencyException(
                $"Goods receipt '{request.Doku}' was modified by another user. Please reload and retry.");
        }

        // Return updated detail (re-query to get fresh RowVersion).
        var detail = await new GetGoodsReceiptByIdQueryHandler(_context)
            .Handle(new GetGoodsReceiptByIdQuery(request.Doku), cancellationToken);

        return detail;
    }
}
