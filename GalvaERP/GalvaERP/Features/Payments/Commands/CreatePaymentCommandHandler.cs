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
        // Validate Supplier exists.
        var supplierExists = await _context.Suppliers
            .AnyAsync(s => s.Kode == request.Kode_Supplier, cancellationToken);

        if (!supplierExists)
        {
            throw new NotFoundException("Supplier not found: " + request.Kode_Supplier);
        }

        // Generate Doku: PAY-{yyyyMMdd}-{nnn}
        var datePart = request.Tgl.ToString("yyyyMMdd");
        var prefix = $"PAY-{datePart}-";

        var todayCount = await _context.Bayars
            .CountAsync(b => b.Doku != null && b.Doku.StartsWith(prefix), cancellationToken);

        var doku = prefix + (todayCount + 1).ToString("D3");

        // Create Bayar header.
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

        // Create SubBayar lines.
        var subBayars = new List<SubBayar>();
        foreach (var item in request.LineItems)
        {
            subBayars.Add(new SubBayar
            {
                Doku = doku,
                Doku_LPB = item.Doku_LPB,
                Doku_PO = item.Doku_PO,
                Doku_Voucher = item.Doku_Voucher,
                Nilai = item.Nilai,
                NilaiBayar = item.NilaiBayar,
                Kode_Supplier = request.Kode_Supplier,
                Kode_Valas = request.Kode_Valas,
                Kurs = request.Kurs,
                Tgl = request.Tgl,
                EntryDate = DateTime.UtcNow,
            });
        }

        _context.Bayars.Add(bayar);
        _context.SubBayars.AddRange(subBayars);
        await _context.SaveChangesAsync(cancellationToken);

        return doku;
    }
}
