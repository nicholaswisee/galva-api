using GalvaERP.Common.Exceptions;
using GalvaERP.Domain.Entities;
using GalvaERP.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GalvaERP.Features.GoodsReceipts.Commands;

public class CreateGoodsReceiptCommandHandler : IRequestHandler<CreateGoodsReceiptCommand, string>
{
    private readonly AppDbContext _context;

    public CreateGoodsReceiptCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<string> Handle(CreateGoodsReceiptCommand request, CancellationToken cancellationToken)
    {
        // Validate PO exists and is Confirmed (STS = "1").
        var po = await _context.POs
            .FirstOrDefaultAsync(p => p.Doku == request.Doku_PO, cancellationToken);

        if (po is null)
        {
            throw new NotFoundException("Purchase order not found: " + request.Doku_PO);
        }

        if (po.STS != "1")
        {
            throw new DomainException("PO must be Confirmed (STS=1). Current STS: " + po.STS);
        }

        // Aggregate PO qty per Kode_Brg so we can validate received qty.
        var poQtyByBrg = await _context.SubPOs
            .Where(s => s.Doku == request.Doku_PO && s.Kode_Brg != null)
            .GroupBy(s => s.Kode_Brg!)
            .Select(g => new
            {
                Kode_Brg = g.Key,
                TotalQty = g.Sum(x => x.Jumlah ?? 0.0),
            })
            .ToDictionaryAsync(x => x.Kode_Brg, x => x.TotalQty, cancellationToken);

        // Aggregate previously received qty per Kode_Brg from existing LPBs for this PO.
        var previousReceiptQtyByBrg = await _context.SubLPBs
            .Where(s => s.Doku_PO == request.Doku_PO && s.Kode_Brg != null)
            .GroupBy(s => s.Kode_Brg!)
            .Select(g => new
            {
                Kode_Brg = g.Key,
                TotalQty = g.Sum(x => x.Jumlah ?? 0.0),
            })
            .ToDictionaryAsync(x => x.Kode_Brg, x => x.TotalQty, cancellationToken);

        // Validate received qty ≤ PO qty per Kode_Brg.
        var requestedByBrg = request.LineItems
            .GroupBy(li => li.Kode_Brg)
            .ToDictionary(g => g.Key, g => g.Sum(x => x.Jumlah));

        foreach (var (kodeBrg, requestedQty) in requestedByBrg)
        {
            var poQty = poQtyByBrg.GetValueOrDefault(kodeBrg, 0.0);
            var alreadyReceived = previousReceiptQtyByBrg.GetValueOrDefault(kodeBrg, 0.0);
            var newTotal = alreadyReceived + requestedQty;
            if (newTotal > poQty + 0.0001)
            {
                throw new DomainException(
                    $"Received quantity for item {kodeBrg} exceeds PO qty. PO qty={poQty}, already received={alreadyReceived}, new requested={requestedQty}.");
            }
        }

        // Generate Doku: GR-{yyyyMMdd}-{nnn}
        var datePart = request.Tgl.ToString("yyyyMMdd");
        var prefix = $"GR-{datePart}-";

        var todayCount = await _context.LPBs
            .CountAsync(l => l.Doku != null && l.Doku.StartsWith(prefix), cancellationToken);

        var doku = prefix + (todayCount + 1).ToString("D3");

        // Create LPB header.
        var lpb = new LPB
        {
            Doku = doku,
            Tgl = request.Tgl,
            Doku_PO = request.Doku_PO,
            Kode_Supplier = request.Kode_Supplier,
            SuratJalan = request.SuratJalan,
            Memo = request.Memo,
            STS = "0",
            Status = "RCVD",
            EntryDate = DateTime.UtcNow,
            TglCreate = DateTime.UtcNow,
        };

        // Create SubLPB lines and sum up the nilai.
        double totalNilai = 0.0;
        var subLPBs = new List<SubLPB>();
        foreach (var item in request.LineItems)
        {
            var nilai = item.Jumlah * item.Harga;
            totalNilai += nilai;

            subLPBs.Add(new SubLPB
            {
                Doku = doku,
                Doku_PO = request.Doku_PO,
                Kode_Brg = item.Kode_Brg,
                Jumlah = item.Jumlah,
                Harga = item.Harga,
                Nilai = nilai,
                Kode_Gudang = item.Kode_Gudang,
                Tgl = request.Tgl,
                TglCreate = DateTime.UtcNow,
            });
        }

        lpb.Nilai = totalNilai;

        _context.LPBs.Add(lpb);
        _context.SubLPBs.AddRange(subLPBs);
        await _context.SaveChangesAsync(cancellationToken);

        return doku;
    }
}
