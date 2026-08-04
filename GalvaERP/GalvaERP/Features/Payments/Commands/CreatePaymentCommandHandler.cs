using GalvaERP.Common.Exceptions;
using GalvaERP.Domain.Entities;
using GalvaERP.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GalvaERP.Features.Payments.Commands;

public class CreatePaymentCommandHandler : IRequestHandler<CreatePaymentCommand, string>
{
    private readonly AppDbContext _context;

    public CreatePaymentCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<string> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
    {
        // ponytail: throws-not-sets referential integrity (galva-db has no FKs; API enforces).
        var supplierExists = await _context.Suppliers
            .AnyAsync(s => s.Kode == request.Kode_Supplier, cancellationToken);
        if (!supplierExists)
        {
            throw new NotFoundException("Vendor (Kode_Supplier) not found: " + request.Kode_Supplier);
        }

        // Generate Doku: BAY-{yyyyMMdd}-{nnn}  (canonical Payment prefix; matches Bayar/Hiapt06).
        // ponytail: count-then-assign is racy under concurrent inserts; upgrade to a sequence if
        // throughput warrants it.
        var datePart = request.Tgl.ToString("yyyyMMdd");
        var prefix = $"BAY-{datePart}-";
        var todayCount = await _context.Bayars
            .CountAsync(b => b.Doku != null && b.Doku.StartsWith(prefix), cancellationToken);
        var doku = prefix + (todayCount + 1).ToString("D3");

        // Validate each payment line against its AP invoice and enforce the over-payment guard.
        var invoiceCache = new Dictionary<string, VoucherAP>(StringComparer.OrdinalIgnoreCase);
        var subBayars = new List<SubBayar>(request.LineItems.Count);
        foreach (var item in request.LineItems)
        {
            if (!invoiceCache.TryGetValue(item.Doku_Faktur, out var voucher))
            {
                voucher = await _context.VoucherAPs
                    .AsNoTracking()
                    .FirstOrDefaultAsync(v => v.Doku == item.Doku_Faktur, cancellationToken);
                if (voucher is null)
                {
                    throw new NotFoundException(
                        $"AP invoice (Doku_Faktur) not found: {item.Doku_Faktur}");
                }
                invoiceCache[item.Doku_Faktur] = voucher;
            }

            // Reject cross-vendor applications: payment header supplier must match the invoice supplier.
            if (!string.Equals(voucher.Kode_Supplier, request.Kode_Supplier, StringComparison.OrdinalIgnoreCase))
            {
                throw new DomainException(
                    $"Payment line references invoice '{item.Doku_Faktur}' whose supplier " +
                    $"'{voucher.Kode_Supplier}' does not match the payment vendor '{request.Kode_Supplier}'.");
            }

            // Over-payment guard: cumulative paid (existing SubBayar.TotalNilai, Hapus IS NULL)
            // plus this line must not exceed the AP invoice header value (VoucherAP.Nilai).
            // ponytail: aggregate per-call; a cross-transaction cache would tighten this when needed.
            var invoiceTotalValue = voucher.Nilai ?? 0d;
            var alreadyPaid = await _context.SubBayars
                .AsNoTracking()
                .Where(s => s.Doku_Faktur == item.Doku_Faktur && s.Hapus == null)
                .SumAsync(s => s.TotalNilai ?? 0d, cancellationToken);
            var remaining = invoiceTotalValue - alreadyPaid;
            if (item.TotalNilai > remaining + 1e-6)
            {
                throw new DomainException(
                    $"Payment for invoice {item.Doku_Faktur} exceeds outstanding balance. " +
                    $"Invoice={item.Doku_Faktur}, Nilai={invoiceTotalValue:0.##}, already-paid={alreadyPaid:0.##}, " +
                    $"remaining={remaining:0.##}, requested={item.TotalNilai:0.##}.");
            }

            subBayars.Add(new SubBayar
            {
                Doku = doku,
                Doku_Faktur = item.Doku_Faktur,
                Doku_LPB = item.Doku_LPB,
                Kode_Supplier = request.Kode_Supplier,
                Nilai = item.Nilai,
                TotalNilai = item.TotalNilai,
                Kode_Valas = request.Kode_Valas,
                Kurs = request.Kurs,
                Tgl = request.Tgl,
                EntryDate = DateTime.UtcNow,
            });
        }

        var bayar = new Bayar
        {
            Doku = doku,
            Tgl = request.Tgl,
            Kode_Supplier = request.Kode_Supplier,
            Kode_BankSupplier = request.Kode_BankSupplier,
            Keterangan = request.Keterangan,
            NilaiKas = request.NilaiKas,
            NilaiGiro = request.NilaiGiro,
            NilMuka = 0,
            STS = "0",
            Kode_Valas = request.Kode_Valas,
            Kurs = request.Kurs,
            EntryDate = DateTime.UtcNow,
        };

        _context.Bayars.Add(bayar);
        _context.SubBayars.AddRange(subBayars);
        await _context.SaveChangesAsync(cancellationToken);

        return doku;
    }
}