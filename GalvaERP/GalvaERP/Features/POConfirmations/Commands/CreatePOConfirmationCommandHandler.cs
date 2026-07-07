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

public class CreatePOConfirmationCommandHandler : IRequestHandler<CreatePOConfirmationCommand, POConfirmationDetailDto>
{
    private readonly AppDbContext _context;

    public CreatePOConfirmationCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<POConfirmationDetailDto> Handle(CreatePOConfirmationCommand request, CancellationToken cancellationToken)
    {
        var po = await _context.POs
            .FirstOrDefaultAsync(p => p.Doku == request.Doku_PO, cancellationToken);

        if (po is null)
        {
            throw new NotFoundException($"Purchase Order '{request.Doku_PO}' was not found.");
        }

        if (po.STS != "0")
        {
            throw new DomainException($"PO must be Pending (STS=0) to confirm. Current STS: {po.STS}");
        }

        var subPOs = await _context.SubPOs
            .Where(s => s.Doku == request.Doku_PO)
            .ToListAsync(cancellationToken);

        var subPOById = subPOs.ToDictionary(s => s.id_sub_po);

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
            var alreadyConfirmed = subPO.JumlahKonfirm ?? 0;
            var remaining = ordered - alreadyConfirmed;
            var confirmQty = line.Jumlah ?? 0;

            if (confirmQty > remaining + 0.0001)
            {
                throw new DomainException(
                    $"Confirmed quantity for {line.Kode_Brg} exceeds remaining PO qty. Ordered={ordered}, already confirmed={alreadyConfirmed}, remaining={remaining}, requested={confirmQty}.");
            }
        }

        var prefix = $"PCF-{request.Tgl:yyyyMMdd}-";
        var todayCount = await _context.POConfirmations
            .Where(p => p.Doku != null && p.Doku.StartsWith(prefix))
            .CountAsync(cancellationToken);

        var doku = $"{prefix}{(todayCount + 1):000}";

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
                Doku = doku,
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

        var confirmation = new POConfirmation
        {
            Doku = doku,
            Tgl = request.Tgl,
            Doku_PO = request.Doku_PO,
            Kode_Supplier = po.Kode_Supplier,
            Kode_dept = po.Kode_dept,
            Kode_Valas = po.Kode_Valas,
            Kurs = po.Kurs,
            ContactPr = request.ContactPr,
            Psd = request.Psd,
            Etd = request.Etd,
            Memo = request.Memo,
            Nilai = total,
            PPN = vat,
            Diskon = disc,
            STS = "0",
            EntryDate = DateTime.Now
        };

        _context.POConfirmations.Add(confirmation);
        _context.SubPOConfirmations.AddRange(confirmationLines);

        var allFullyConfirmed = subPOs.All(s => (s.JumlahKonfirm ?? 0) >= (s.Jumlah ?? 0) - 0.0001);
        if (allFullyConfirmed)
        {
            po.STS = "1";
        }

        await _context.SaveChangesAsync(cancellationToken);

        return await RequeryDetail(doku, cancellationToken);
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
