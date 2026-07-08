using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GalvaERP.Common.Exceptions;
using GalvaERP.Domain.Entities;
using GalvaERP.Features.POConfirmations.DTOs;
using GalvaERP.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GalvaERP.Features.POConfirmations.Commands;

public class UpdatePOConfirmationCommandHandler : IRequestHandler<UpdatePOConfirmationCommand, POConfirmationDetailDto>
{
    private readonly AppDbContext _context;

    public UpdatePOConfirmationCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<POConfirmationDetailDto> Handle(UpdatePOConfirmationCommand request, CancellationToken cancellationToken)
    {
        var confirmation = await _context.POConfirmations
            .FirstOrDefaultAsync(p => p.Doku == request.Doku, cancellationToken);

        if (confirmation is null)
        {
            throw new NotFoundException($"PO Confirmation '{request.Doku}' was not found.");
        }

        if (!confirmation.RowVersion.SequenceEqual(request.IfMatchRowVersion))
        {
            throw new ConcurrencyException(
                $"PO Confirmation '{request.Doku}' was modified by another user. Please reload and try again.");
        }

        var po = await _context.POs
            .FirstOrDefaultAsync(p => p.Doku == request.Doku_PO, cancellationToken);

        if (po is null)
        {
            throw new NotFoundException($"Purchase Order '{request.Doku_PO}' was not found.");
        }

        if (po.STS != "0")
        {
            throw new DomainException(
                $"PO Confirmation '{request.Doku}' can only be updated while parent PO STS is '0' (Pending). Current parent STS: {po.STS}.");
        }

        var subPOs = await _context.SubPOs
            .Where(s => s.Doku == request.Doku_PO)
            .ToListAsync(cancellationToken);

        var subPOById = subPOs.ToDictionary(s => s.id_sub_po);

        var oldLines = await _context.SubPOConfirmations
            .Where(sc => sc.Doku == request.Doku)
            .ToListAsync(cancellationToken);

        // Reverse old quantities from parent SubPO.JumlahKonfirm (captured in a map first).
        var oldQtyBySubPo = oldLines
            .GroupBy(l => l.id_sub_po ?? 0L)
            .ToDictionary(g => g.Key, g => g.Sum(x => x.Jumlah ?? 0d));

        foreach (var line in request.LineItems)
        {
            var idSubPo = line.id_sub_po ?? 0;
            if (idSubPo == 0)
            {
                throw new DomainException("id_sub_po is required on every line item.");
            }

            if (!subPOById.TryGetValue(idSubPo, out var subPO))
            {
                throw new DomainException($"PO line {idSubPo} not found.");
            }

            if (subPO.Kode_Brg != line.Kode_Brg)
            {
                throw new DomainException($"Line {idSubPo} does not match item {line.Kode_Brg}.");
            }

            var ordered = subPO.Jumlah ?? 0;
            var currentlyConfirmed = subPO.JumlahKonfirm ?? 0;
            oldQtyBySubPo.TryGetValue(idSubPo, out var oldQty);
            var remaining = ordered - (currentlyConfirmed - oldQty);
            var confirmQty = line.Jumlah ?? 0;

            if (confirmQty > remaining + 0.0001)
            {
                throw new DomainException(
                    $"Confirmed quantity for {line.Kode_Brg} exceeds remaining PO qty. Ordered={ordered}, already confirmed by other confirmations={currentlyConfirmed - oldQty}, remaining={remaining}, requested={confirmQty}.");
            }
        }

        // Reverse old quantities from parent SubPOs.
        foreach (var (idSubPo, qty) in oldQtyBySubPo)
        {
            if (qty == 0d) continue;
            if (subPOById.TryGetValue(idSubPo, out var subPO))
            {
                subPO.JumlahKonfirm = (subPO.JumlahKonfirm ?? 0) - qty;
            }
        }

        // Remove old SubPOConfirmation rows.
        _context.SubPOConfirmations.RemoveRange(oldLines);

        double gross = 0d;
        double disc = 0d;
        var confirmationLines = new List<SubPOConfirmation>();

        foreach (var line in request.LineItems)
        {
            var idSubPo = line.id_sub_po ?? 0;
            var subPO = subPOById[idSubPo];
            var lineGross = (line.Jumlah ?? 0) * (line.Harga ?? 0);
            var lineNet = lineGross;

            gross += lineGross;

            confirmationLines.Add(new SubPOConfirmation
            {
                Doku = request.Doku,
                id_sub_po = idSubPo,
                Kode_Brg = line.Kode_Brg,
                Jumlah = line.Jumlah,
                Harga = line.Harga,
                Total = lineNet,
                Kode_Gudang = line.Kode_Gudang,
                Note = line.Note,
                EntryDate = DateTime.Now
            });

            subPO.JumlahKonfirm = (subPO.JumlahKonfirm ?? 0) + (line.Jumlah ?? 0);
        }

        var net = gross - disc;
        var vat = net * 0.12d;
        var total = net + vat;

        confirmation.Tgl = request.Tgl;
        confirmation.Doku_PO = request.Doku_PO;
        confirmation.Kode_Supplier = po.Kode_Supplier;
        confirmation.Kode_dept = po.Kode_dept;
        confirmation.Kode_Valas = po.Kode_Valas;
        confirmation.Kurs = po.Kurs;
        confirmation.ContactPr = request.ContactPr;
        confirmation.Psd = request.Psd;
        confirmation.Etd = request.Etd;
        confirmation.Memo = request.Memo;
        confirmation.Nilai = total;
        confirmation.PPN = vat;
        confirmation.Diskon = disc;

        _context.SubPOConfirmations.AddRange(confirmationLines);

        // Recompute parent PO STS based on all SubPOs (including any non-zero oldQty sub-pos no longer in this confirmation).
        var allFullyConfirmed = subPOs.All(s => (s.JumlahKonfirm ?? 0) >= (s.Jumlah ?? 0) - 0.0001);
        po.STS = allFullyConfirmed ? "1" : "0";

        _context.Entry(confirmation).Property(p => p.RowVersion).OriginalValue = request.IfMatchRowVersion;

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConcurrencyException(
                $"PO Confirmation '{request.Doku}' was modified by another user. Please reload and try again.");
        }

        return await RequeryDetail(request.Doku, cancellationToken);
    }

    private async Task<POConfirmationDetailDto> RequeryDetail(string doku, CancellationToken cancellationToken)
    {
        var query =
            from pc in _context.POConfirmations.AsNoTracking()
            join s in _context.Suppliers.AsNoTracking() on pc.Kode_Supplier equals s.Kode into suppliers
            from s in suppliers.DefaultIfEmpty()
            where pc.Doku == doku
            select new
            {
                pc.Doku,
                pc.Tgl,
                pc.Doku_PO,
                pc.Kode_Supplier,
                SupplierName = s != null ? s.Nama : null,
                pc.Kode_dept,
                pc.Kode_Valas,
                pc.Kurs,
                pc.ContactPr,
                pc.Psd,
                pc.Etd,
                pc.Memo,
                pc.Nilai,
                pc.PPN,
                pc.Diskon,
                pc.STS,
                pc.RowVersion
            };

        var pcRow = await query.FirstAsync(cancellationToken);

        var lines = await _context.SubPOConfirmations
            .AsNoTracking()
            .Where(sc => sc.Doku == doku)
            .OrderBy(sc => sc.id_sub_po_confirmation)
            .Select(sc => new POConfirmationLineDto(
                sc.id_sub_po_confirmation,
                sc.id_sub_po,
                sc.Kode_Brg,
                sc.Jumlah,
                sc.Harga,
                sc.Total,
                sc.Kode_Gudang,
                sc.Note))
            .ToListAsync(cancellationToken);

        return new POConfirmationDetailDto(
            pcRow.Doku ?? string.Empty,
            pcRow.Tgl,
            pcRow.Doku_PO,
            pcRow.Kode_Supplier,
            pcRow.SupplierName,
            pcRow.Kode_dept,
            pcRow.Kode_Valas,
            pcRow.Kurs,
            pcRow.ContactPr,
            pcRow.Psd,
            pcRow.Etd,
            pcRow.Memo,
            pcRow.Nilai,
            pcRow.PPN,
            pcRow.Diskon,
            pcRow.STS,
            Convert.ToBase64String(pcRow.RowVersion),
            lines);
    }
}
