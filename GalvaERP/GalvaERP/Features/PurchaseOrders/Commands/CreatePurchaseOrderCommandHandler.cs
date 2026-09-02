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

namespace GalvaERP.Features.PurchaseOrders.Commands;

public class CreatePurchaseOrderCommandHandler : IRequestHandler<CreatePurchaseOrderCommand, string>
{
    private readonly AppDbContext _context;

    public CreatePurchaseOrderCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<string> Handle(CreatePurchaseOrderCommand request, CancellationToken cancellationToken)
    {
        var supplier = await _context.Suppliers
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Kode == request.Kode_Supplier, cancellationToken);

        if (supplier is null)
        {
            throw new DomainException($"Supplier '{request.Kode_Supplier}' does not exist.");
        }

        var prefix = $"POS-{request.Tgl:yyyyMMdd}-";
        var todayCount = await _context.POSems
            .Where(p => p.Doku != null && p.Doku.StartsWith(prefix))
            .CountAsync(cancellationToken);

        var doku = $"{prefix}{(todayCount + 1):000}";

        var po = new POSem
        {
            Doku = doku,
            Tgl = request.Tgl,
            Kode_Supplier = request.Kode_Supplier,
            Kode_dept = request.Kode_dept,
            Memo = request.Memo,
            Kode_Valas = request.Kode_Valas,
            Kurs = request.Kurs,
            Syarat = request.Syarat,
            STS = "0",
            PPN = request.Ppn,
            Diskon = request.Diskon,
            DPPNilaiLain = request.DppNilaiLain,
            PPnTunai = request.PPnTunai,
            EntryDate = DateTime.Now,
            Wkt = DateTime.Now
        };

        double gross = 0d;
        double disc = 0d;
        var subPos = new List<SubPOSem>();

        foreach (var line in request.LineItems)
        {
            var lineGross = line.Jumlah * line.Harga;
            var lineDisc = line.Disc;
            var lineNet = lineGross - lineDisc;

            gross += lineGross;
            disc += lineDisc;

            subPos.Add(new SubPOSem
            {
                Doku = doku,
                Kode_Brg = line.Kode_Brg,
                Merk = line.Merk,
                Model = line.Model,
                Satuan = line.Satuan,
                Jumlah = line.Jumlah,
                Harga = line.Harga,
                DiscPct = line.DiscPct,
                Diskon = lineDisc,
                Total = lineNet,
                Kode_Gudang = line.Kode_Gudang,
                Alias = line.Alias,
                Keterangan = line.Note,
                TglKirim = string.IsNullOrEmpty(line.Schedule) ? null : DateTime.Parse(line.Schedule),
                Kode_Valas = line.Kode_Valas ?? request.Kode_Valas,
                PPN = line.Ppn,
                Kode_Dept = request.Kode_dept,
                Tgl = request.Tgl,
                EntryDate = DateTime.Now
            });
        }

        var net = gross - disc;
        var dpp = request.DppNilaiLain > 0 ? request.DppNilaiLain : net;
        var vat = dpp * (request.Ppn / 100d);
        var total = dpp + vat + request.PPnTunai;

        po.Nilai = total;

        _context.POSems.Add(po);
        _context.SubPOSems.AddRange(subPos);

        await _context.SaveChangesAsync(cancellationToken);

        return doku;
    }
}
