using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GalvaERP.Common.Exceptions;
using GalvaERP.Domain.Entities;
using GalvaERP.Features.PurchaseOrders.DTOs;
using GalvaERP.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GalvaERP.Features.PurchaseOrders.Commands;

public class UpdatePurchaseOrderCommandHandler : IRequestHandler<UpdatePurchaseOrderCommand, PODetailDto>
{
    private readonly AppDbContext _context;

    public UpdatePurchaseOrderCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PODetailDto> Handle(UpdatePurchaseOrderCommand request, CancellationToken cancellationToken)
    {
        var po = await _context.POs
            .FirstOrDefaultAsync(p => p.Doku == request.Doku, cancellationToken);

        if (po is null)
        {
            throw new NotFoundException($"Purchase Order '{request.Doku}' was not found.");
        }

        if (po.Hapus == "Y")
        {
            throw new NotFoundException($"Purchase Order '{request.Doku}' was not found.");
        }

        if (!po.RowVersion.SequenceEqual(request.IfMatchRowVersion))
        {
            throw new ConcurrencyException(
                $"Purchase Order '{request.Doku}' was modified by another user. Please reload and try again.");
        }

        if (po.STS != "0")
        {
            throw new DomainException(
                $"Purchase Order '{request.Doku}' can only be updated while STS is '0' (Pending).");
        }

        po.Tgl = request.Tgl;
        po.Kode_Supplier = request.Kode_Supplier;
        po.Kode_dept = request.Kode_dept;
        po.Memo = request.Memo;
        po.Kode_Valas = request.Kode_Valas;
        po.Kurs = request.Kurs;
        po.Syarat = request.Syarat;
        po.PPN = request.Ppn;
        po.Diskon = request.Diskon;
        po.DPPNilaiLain = request.DppNilaiLain;
        po.PPnTunai = request.PPnTunai;

        var existingLines = await _context.SubPOs
            .Where(sp => sp.Doku == request.Doku)
            .ToListAsync(cancellationToken);
        _context.SubPOs.RemoveRange(existingLines);

        double gross = 0d;
        double disc = 0d;
        var newLines = new List<SubPO>();

        foreach (var line in request.LineItems)
        {
            var lineGross = line.Jumlah * line.Harga;
            var lineDisc = line.Disc;
            var lineNet = lineGross - lineDisc;

            gross += lineGross;
            disc += lineDisc;

            newLines.Add(new SubPO
            {
                Doku = request.Doku,
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

        _context.SubPOs.AddRange(newLines);
        _context.Entry(po).Property(p => p.RowVersion).OriginalValue = request.IfMatchRowVersion;

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConcurrencyException(
                $"Purchase Order '{request.Doku}' was modified by another user. Please reload and try again.");
        }

        var refreshed = await _context.POs.AsNoTracking()
            .FirstAsync(p => p.Doku == request.Doku, cancellationToken);

        var supplier = await _context.Suppliers.AsNoTracking()
            .FirstOrDefaultAsync(s => s.Kode == refreshed.Kode_Supplier, cancellationToken);

        var lines = await _context.SubPOs
            .AsNoTracking()
            .Where(sp => sp.Doku == request.Doku)
            .OrderBy(sp => sp.id_sub_po)
            .Select(sp => new PODetailLineDto(
                sp.id_sub_po,
                sp.Kode_Brg,
                sp.Merk,
                sp.Model,
                sp.Satuan,
                sp.Jumlah,
                sp.Harga,
                sp.DiscPct,
                sp.Diskon,
                sp.Total,
                sp.JumlahKonfirm ?? 0,
                sp.Kode_Gudang,
                sp.Alias,
                sp.Keterangan,
                sp.TglKirim.HasValue ? sp.TglKirim.Value.ToString("yyyy-MM-dd") : null))
            .ToListAsync(cancellationToken);

        return new PODetailDto(
            refreshed.Doku ?? string.Empty,
            refreshed.Tgl,
            refreshed.Kode_Supplier,
            supplier?.Nama,
            refreshed.Kode_dept,
            refreshed.Kode_Valas,
            refreshed.Kurs,
            refreshed.Nilai,
            refreshed.DPPNilaiLain,
            refreshed.PPN,
            refreshed.PPnTunai,
            refreshed.Diskon,
            refreshed.Syarat,
            refreshed.STS,
            refreshed.Memo,
            Convert.ToBase64String(refreshed.RowVersion),
            lines);
    }
}
