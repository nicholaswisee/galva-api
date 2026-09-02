using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
        var po = await _context.POs
            .FirstOrDefaultAsync(p => p.Doku == request.Doku_PO, cancellationToken);

        if (po is null || po.Hapus != null)
        {
            throw new NotFoundException($"Purchase Order '{request.Doku_PO}' was not found.");
        }

        if (po.STS == "9")
        {
            throw new DomainException($"Purchase Order '{request.Doku_PO}' is cancelled and cannot be received against.");
        }

        // Load PO lines keyed by id_sub_po (or fallback by Kode_Brg)
        var poLines = await _context.SubPOs
            .Where(sp => sp.Doku == request.Doku_PO)
            .ToListAsync(cancellationToken);

        var ambiguousSku = poLines
            .Where(line => !string.IsNullOrEmpty(line.Kode_Brg))
            .GroupBy(line => line.Kode_Brg!)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .ToList();
        if (ambiguousSku.Count > 0)
        {
            throw new DomainException(
                $"Purchase Order '{request.Doku_PO}' has duplicate item codes and cannot be received safely: {string.Join(", ", ambiguousSku)}.");
        }

        var poLineById = poLines
            .Where(sp => sp.id_sub_po > 0)
            .ToDictionary(sp => sp.id_sub_po);

        var poLineByKodeBrg = poLines
            .Where(sp => !string.IsNullOrEmpty(sp.Kode_Brg))
            .GroupBy(sp => sp.Kode_Brg!)
            .ToDictionary(g => g.Key, g => g.Single());

        // Aggregate previously received quantity per PO line
        var previousReceiptsByPOLine = await _context.SubLPBs
            .Where(s => s.Doku_PO == request.Doku_PO && s.Hapus == null)
            .GroupBy(s => s.Kode_Brg)
            .Select(g => new
            {
                Kode_Brg = g.Key,
                TotalQty = g.Sum(x => x.Jumlah ?? 0.0),
            })
            .ToListAsync(cancellationToken);

        var previousReceiptMap = previousReceiptsByPOLine
            .Where(r => !string.IsNullOrEmpty(r.Kode_Brg))
            .ToDictionary(r => r.Kode_Brg!, r => r.TotalQty);

        foreach (var item in request.LineItems)
        {
            if (item.ResolvedSubPOId > 0)
            {
                if (!poLineById.TryGetValue(item.ResolvedSubPOId, out var lineById))
                {
                    throw new DomainException($"PO line {item.ResolvedSubPOId} was not found in PO '{request.Doku_PO}'.");
                }

                if (!string.Equals(lineById.Kode_Brg, item.Kode_Brg, StringComparison.Ordinal))
                {
                    throw new DomainException(
                        $"PO line {item.ResolvedSubPOId} does not match item '{item.Kode_Brg}'.");
                }
            }
            else if (!poLineByKodeBrg.ContainsKey(item.Kode_Brg))
            {
                throw new DomainException($"PO line for item '{item.Kode_Brg}' was not found in PO '{request.Doku_PO}'.");
            }
        }

        var requestedQtyByItem = request.LineItems
            .GroupBy(item => item.Kode_Brg)
            .ToDictionary(group => group.Key, group => group.Sum(item => item.Jumlah));

        foreach (var (kodeBrg, requestedQty) in requestedQtyByItem)
        {
            var poLine = poLineByKodeBrg[kodeBrg];
            var orderedQty = poLine.Jumlah ?? 0.0;
            var alreadyReceived = previousReceiptMap.GetValueOrDefault(kodeBrg, 0.0);
            var remaining = orderedQty - alreadyReceived;

            if (requestedQty > remaining + 0.0001)
            {
                throw new DomainException(
                    $"Received quantity for item '{kodeBrg}' exceeds remaining order quantity. " +
                    $"PO={request.Doku_PO}, ordered={orderedQty}, already received={alreadyReceived}, remaining={remaining}, requested={requestedQty}.");
            }
        }

        // Generate Doku: GR-{yyyyMMdd}-{nnn}
        var datePart = request.Tgl.ToString("yyyyMMdd");
        var prefix = $"GR-{datePart}-";

        var todayCount = await _context.LPBs
            .CountAsync(l => l.Doku != null && l.Doku.StartsWith(prefix), cancellationToken);

        var doku = prefix + (todayCount + 1).ToString("D3");

        // Create LPB header
        var lpb = new LPB
        {
            Doku = doku,
            Tgl = request.Tgl,
            Doku_PO = request.Doku_PO,
            Kode_Supplier = request.Kode_Supplier ?? po.Kode_Supplier,
            Kode_Valas = request.Kode_Valas ?? po.Kode_Valas,
            Kurs = request.Kurs ?? po.Kurs,
            SuratJalan = request.SuratJalan,
            Memo = request.Memo,
            STS = "0",
            Status = "RCVD",
            EntryDate = DateTime.UtcNow,
            TglCreate = DateTime.UtcNow,
        };

        // Create SubLPB lines and update SubPO.JumlahKirim
        double totalNilai = 0.0;
        var subLPBs = new List<SubLPB>();
        foreach (var item in request.LineItems)
        {
            var nilai = item.Jumlah * item.Harga;
            totalNilai += nilai;

            var poLine = item.ResolvedSubPOId > 0
                ? poLineById[item.ResolvedSubPOId]
                : poLineByKodeBrg[item.Kode_Brg];
            poLine.JumlahKirim = (poLine.JumlahKirim ?? 0.0) + item.Jumlah;

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

        // Check if all lines for the PO are fully received
        var allFullyReceived = poLines.All(p => (p.JumlahKirim ?? 0.0) >= (p.Jumlah ?? 0.0) - 0.0001);
        if (allFullyReceived)
        {
            po.STS = "2"; // Fully Received / Processed
        }
        else
        {
            po.STS = "1"; // Partially Received / Active
        }

        _context.LPBs.Add(lpb);
        _context.SubLPBs.AddRange(subLPBs);
        await _context.SaveChangesAsync(cancellationToken);

        return doku;
    }
}
