using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

        if (gr is null || gr.Hapus != null)
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
            .Where(s => s.Doku == request.Doku && s.Hapus == null)
            .ToListAsync(cancellationToken);

        // Revert received quantities on PO lines
        if (!string.IsNullOrEmpty(gr.Doku_PO))
        {
            var poLines = await _context.SubPOs
                .Where(sp => sp.Doku == gr.Doku_PO)
                .ToListAsync(cancellationToken);

            var poLineByKodeBrg = poLines
                .Where(sp => !string.IsNullOrEmpty(sp.Kode_Brg))
                .GroupBy(sp => sp.Kode_Brg!)
                .ToDictionary(g => g.Key, g => g.First());

            foreach (var sub in subRows)
            {
                if (!string.IsNullOrEmpty(sub.Kode_Brg) && poLineByKodeBrg.TryGetValue(sub.Kode_Brg, out var poLine))
                {
                    var currentKirim = poLine.JumlahKirim ?? 0.0;
                    var subQty = sub.Jumlah ?? 0.0;
                    poLine.JumlahKirim = Math.Max(0.0, currentKirim - subQty);
                }
                sub.Hapus = "Y";
            }

            var po = await _context.POs.FirstOrDefaultAsync(p => p.Doku == gr.Doku_PO, cancellationToken);
            if (po != null && po.STS == "2")
            {
                po.STS = "1"; // Revert status to Active/Partial
            }
        }
        else
        {
            foreach (var sub in subRows)
            {
                sub.Hapus = "Y";
            }
        }

        gr.Hapus = "Y";

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
