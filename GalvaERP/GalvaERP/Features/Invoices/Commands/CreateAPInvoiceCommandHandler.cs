using GalvaERP.Common.Exceptions;
using GalvaERP.Domain.Entities;
using GalvaERP.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GalvaERP.Features.Invoices.Commands;

public class CreateAPInvoiceCommandHandler : IRequestHandler<CreateAPInvoiceCommand, string>
{
    private readonly AppDbContext _context;

    public CreateAPInvoiceCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<string> Handle(CreateAPInvoiceCommand request, CancellationToken cancellationToken)
    {
        // Validate Supplier exists.
        var supplierExists = await _context.Suppliers
            .AnyAsync(s => s.Kode == request.Kode_Supplier, cancellationToken);

        if (!supplierExists)
        {
            throw new NotFoundException("Supplier not found: " + request.Kode_Supplier);
        }

        // Validate all GRLinks reference existing LPBs.
        var grNumbers = request.GRLinks.Select(g => g.Doku_LPB).Distinct().ToList();
        var existingGRs = await _context.LPBs
            .Where(l => l.Doku != null && grNumbers.Contains(l.Doku))
            .Select(l => l.Doku)
            .ToListAsync(cancellationToken);

        var missing = grNumbers.Except(existingGRs).ToList();
        if (missing.Count > 0)
        {
            throw new NotFoundException("Goods receipt(s) not found: " + string.Join(", ", missing));
        }

        // 3-way match: Nilai must equal sum of GR link NilaiLPB.
        var sumNilaiLPB = request.GRLinks.Sum(g => g.NilaiLPB);
        if (Math.Abs(request.Nilai - sumNilaiLPB) > 0.01)
        {
            throw new DomainException(
                $"3-way match failed. Voucher Nilai ({request.Nilai}) must equal sum of linked GR NilaiLPB ({sumNilaiLPB}).");
        }

        // Generate Doku: VAP-{yyyyMMdd}-{nnn}
        var datePart = request.Tgl.ToString("yyyyMMdd");
        var prefix = $"VAP-{datePart}-";

        var todayCount = await _context.VoucherAPs
            .CountAsync(v => v.Doku != null && v.Doku.StartsWith(prefix), cancellationToken);

        var doku = prefix + (todayCount + 1).ToString("D3");

        // Create VoucherAP header.
        var voucher = new VoucherAP
        {
            Doku = doku,
            TglDoku = request.Tgl,
            Kode_Supplier = request.Kode_Supplier,
            Kode_Dept = request.Kode_Dept,
            Kode_Valas = request.Kode_Valas,
            Kurs = request.Kurs,
            Nilai = request.Nilai,
            PPn = request.PPn,
            Diskon = request.Diskon,
            Misc = request.Misc,
            Doku_FP = request.Doku_FP,
            Tgl_FP = request.Tgl_FP,
            Keterangan = request.Keterangan,
            SourceType = "GR",
            STS = "0",
            EntryDate = DateTime.UtcNow,
        };

        // Create SubVoucherAP entries for each GR link.
        var subVouchers = new List<SubVoucherAP>();
        foreach (var link in request.GRLinks)
        {
            subVouchers.Add(new SubVoucherAP
            {
                Doku = doku,
                SourceType = "GR",
                Doku_LPB = link.Doku_LPB,
                NilaiLPB = link.NilaiLPB,
                Nilai = link.NilaiLPB,
                Kode_Supplier = request.Kode_Supplier,
                Kode_Valas = request.Kode_Valas,
                Kurs = request.Kurs,
                Tgl = request.Tgl,
                EntryDate = DateTime.UtcNow,
            });
        }

        _context.VoucherAPs.Add(voucher);
        _context.SubVoucherAPs.AddRange(subVouchers);
        await _context.SaveChangesAsync(cancellationToken);

        return doku;
    }
}
