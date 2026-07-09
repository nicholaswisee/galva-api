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
        // Load the PO Confirmation this GR is tied to.
        var poConfirmation = await _context.POConfirmations
            .FirstOrDefaultAsync(pc => pc.Doku == request.Doku_PCF, cancellationToken);

        if (poConfirmation is null)
        {
            throw new NotFoundException($"PO Confirmation '{request.Doku_PCF}' was not found.");
        }

        if (poConfirmation.STS == "9")
        {
            throw new DomainException($"PO Confirmation '{request.Doku_PCF}' is cancelled and cannot be received against.");
        }

        if (poConfirmation.Doku_PO != request.Doku_PO)
        {
            throw new DomainException(
                $"PO Confirmation '{request.Doku_PCF}' belongs to PO '{poConfirmation.Doku_PO}', not '{request.Doku_PO}'.");
        }

        // Load PO Confirmation lines keyed by their surrogate id.
        var confirmationLines = await _context.SubPOConfirmations
            .Where(sc => sc.Doku == request.Doku_PCF)
            .ToListAsync(cancellationToken);

        var confirmationLineById = confirmationLines
            .ToDictionary(sc => sc.id_sub_po_confirmation);

        // Aggregate previously received quantity per PO Confirmation line.
        var previousReceiptByConfirmationLine = await _context.SubLPBs
            .Where(s => s.Doku_PCF == request.Doku_PCF && s.id_sub_po_confirmation != null)
            .GroupBy(s => s.id_sub_po_confirmation!.Value)
            .Select(g => new
            {
                id_sub_po_confirmation = g.Key,
                TotalQty = g.Sum(x => x.Jumlah ?? 0.0),
            })
            .ToDictionaryAsync(x => x.id_sub_po_confirmation, x => x.TotalQty, cancellationToken);

        // Validate each requested line against its PO Confirmation line.
        foreach (var item in request.LineItems)
        {
            if (!confirmationLineById.TryGetValue(item.id_sub_po_confirmation, out var confirmationLine))
            {
                throw new DomainException(
                    $"PO Confirmation line {item.id_sub_po_confirmation} was not found in '{request.Doku_PCF}'.");
            }

            if (confirmationLine.Kode_Brg != item.Kode_Brg)
            {
                throw new DomainException(
                    $"PO Confirmation line {item.id_sub_po_confirmation} is for item '{confirmationLine.Kode_Brg}', not '{item.Kode_Brg}'.");
            }

            var confirmedQty = confirmationLine.Jumlah ?? 0.0;
            var alreadyReceived = previousReceiptByConfirmationLine.GetValueOrDefault(item.id_sub_po_confirmation, 0.0);
            var remaining = confirmedQty - alreadyReceived;

            if (item.Jumlah > remaining + 0.0001)
            {
                throw new DomainException(
                    $"Received quantity for item {item.Kode_Brg} exceeds the remaining confirmed quantity. " +
                    $"PO Confirmation={request.Doku_PCF}, line={item.id_sub_po_confirmation}, " +
                    $"confirmed={confirmedQty}, already received={alreadyReceived}, remaining={remaining}, requested={item.Jumlah}.");
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
            Doku_PCF = request.Doku_PCF,
            Kode_Supplier = request.Kode_Supplier ?? poConfirmation.Kode_Supplier,
            Kode_Valas = request.Kode_Valas ?? poConfirmation.Kode_Valas,
            Kurs = request.Kurs ?? poConfirmation.Kurs,
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
                Doku_PCF = request.Doku_PCF,
                id_sub_po_confirmation = item.id_sub_po_confirmation,
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
