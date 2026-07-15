using GalvaERP.Common.Exceptions;
using GalvaERP.Domain.Entities;
using GalvaERP.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GalvaERP.Features.Invoices.Commands;

public class CreatePOBasedAPInvoiceCommandHandler : IRequestHandler<CreatePOBasedAPInvoiceCommand, string>
{
    private readonly AppDbContext _context;

    public CreatePOBasedAPInvoiceCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<string> Handle(CreatePOBasedAPInvoiceCommand request, CancellationToken cancellationToken)
    {
        var supplierExists = await _context.Suppliers
            .AnyAsync(s => s.Kode == request.Kode_Supplier, cancellationToken);

        if (!supplierExists)
        {
            throw new NotFoundException("Supplier not found: " + request.Kode_Supplier);
        }

        var poNumbers = request.POLinks.Select(p => p.Doku_PO).Distinct().ToList();
        var existingPOs = await _context.POs
            .Where(p => p.Doku != null && poNumbers.Contains(p.Doku))
            .Select(p => p.Doku)
            .ToListAsync(cancellationToken);

        var missing = poNumbers.Except(existingPOs).ToList();
        if (missing.Count > 0)
        {
            throw new NotFoundException("Purchase order(s) not found: " + string.Join(", ", missing));
        }

        var sumCostLines = request.CostLines.Sum(c => c.TotalRp);
        if (Math.Abs(request.TotalRp - sumCostLines) > 0.01)
        {
            throw new DomainException(
                $"Total amount ({request.TotalRp}) must equal the sum of cost line totals ({sumCostLines}).");
        }

        var datePart = request.Tgl.ToString("yyyyMMdd");
        var prefix = $"VAP-{datePart}-";

        var todayCount = await _context.VoucherAPs
            .CountAsync(v => v.Doku != null && v.Doku.StartsWith(prefix), cancellationToken);

        var doku = prefix + (todayCount + 1).ToString("D3");

        var voucher = new VoucherAP
        {
            Doku = doku,
            TglDoku = request.Tgl,
            Kode_Supplier = request.Kode_Supplier,
            NOPEN = request.NOPEN,
            TglNopen = request.TglNopen,
            AWB_BL = request.AWB_BL,
            Nilai = request.Amount,
            PPn = request.PPn,
            NilaiLPB = request.TotalRp,
            Keterangan = request.Keterangan,
            TipeBiaya = "PO",
            STS = "0",
            EntryDate = DateTime.UtcNow,
        };

        var subVouchers = new List<SubVoucherAP>();

        foreach (var link in request.POLinks)
        {
            subVouchers.Add(new SubVoucherAP
            {
                Doku = doku,
                TipeBiaya = "PO",
                Doku_PO = link.Doku_PO,
                Tgl = link.Tgl ?? request.Tgl,
                NilaiLPB = link.Amount,
                Nilai = link.Amount,
                PPn = link.Tax,
                Kode_Supplier = request.Kode_Supplier,
                EntryDate = DateTime.UtcNow,
            });
        }

        foreach (var line in request.CostLines)
        {
            subVouchers.Add(new SubVoucherAP
            {
                Doku = doku,
                TipeBiaya = "PO",
                APRef = line.APRef,
                InvoiceNo = line.InvoiceNo,
                TglInvoice = line.TglInvoice,
                NilaiLPB = line.AmountRp,
                Nilai = line.AmountRp,
                PPn = line.PPnRp,
                Kode_Valas = line.Kode_Valas,
                Kurs = line.Rate,
                Doku_FP = line.FakturPajak,
                Tgl_FP = line.Tgl_FP,
                Kode_Supplier = request.Kode_Supplier,
                Tgl = line.TglInvoice ?? request.Tgl,
                EntryDate = DateTime.UtcNow,
            });
        }

        _context.VoucherAPs.Add(voucher);
        _context.SubVoucherAPs.AddRange(subVouchers);
        await _context.SaveChangesAsync(cancellationToken);

        return doku;
    }
}
