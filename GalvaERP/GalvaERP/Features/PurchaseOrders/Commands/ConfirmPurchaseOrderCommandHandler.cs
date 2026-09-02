using GalvaERP.Common.Exceptions;
using GalvaERP.Domain.Entities;
using GalvaERP.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GalvaERP.Features.PurchaseOrders.Commands;

public class ConfirmPurchaseOrderCommandHandler : IRequestHandler<ConfirmPurchaseOrderCommand, string>
{
    private readonly AppDbContext _context;

    public ConfirmPurchaseOrderCommandHandler(AppDbContext context) => _context = context;

    public async Task<string> Handle(ConfirmPurchaseOrderCommand request, CancellationToken cancellationToken)
    {
        var draft = await _context.POSems
            .FirstOrDefaultAsync(po => po.Doku == request.Doku_POSem && po.Hapus != "Y", cancellationToken)
            ?? throw new NotFoundException($"Purchase order draft '{request.Doku_POSem}' was not found.");

        if (draft.STS != "0")
        {
            throw new DomainException($"Purchase order draft '{request.Doku_POSem}' is not pending.");
        }

        var draftLines = await _context.SubPOSems
            .Where(line => line.Doku == request.Doku_POSem)
            .ToDictionaryAsync(line => line.id_sub_posem, cancellationToken);

        var draftLineIds = request.LineItems.Select(line => line.id_sub_posem).ToList();
        if (draftLineIds.Distinct().Count() != draftLineIds.Count)
        {
            throw new DomainException("Duplicate draft line IDs are not allowed.");
        }

        foreach (var line in request.LineItems)
        {
            if (!draftLines.TryGetValue(line.id_sub_posem, out var draftLine) || draftLine.Kode_Brg != line.Kode_Brg)
            {
                throw new DomainException($"Draft line {line.id_sub_posem} does not match item '{line.Kode_Brg}'.");
            }

            var remaining = (draftLine.Jumlah ?? 0) - (draftLine.JumlahKonfirm ?? 0);
            if (line.Jumlah > remaining + 0.0001)
            {
                throw new DomainException(
                    $"Confirmed quantity for item '{line.Kode_Brg}' exceeds the remaining draft quantity. " +
                    $"Draft={request.Doku_POSem}, line={line.id_sub_posem}, remaining={remaining}, requested={line.Jumlah}.");
            }
        }

        var prefix = $"PO-{request.Tgl:yyyyMMdd}-";
        var todayCount = await _context.POs
            .CountAsync(po => po.Doku != null && po.Doku.StartsWith(prefix), cancellationToken);
        var doku = $"{prefix}{todayCount + 1:000}";

        var priorConfirmations = await _context.POs
            .Where(po => po.Doku_POSem == request.Doku_POSem && po.Hapus != "Y")
            .ToListAsync(cancellationToken);
        var sourceBasis = draftLines.Values.Sum(draftLine => draftLine.Total ??
            ((draftLine.Jumlah ?? 0) * (draftLine.Harga ?? 0) - (draftLine.Diskon ?? 0)));
        var confirmationBasis = 0d;

        double gross = 0;
        var lines = new List<SubPO>(request.LineItems.Count);
        foreach (var line in request.LineItems)
        {
            var draftLine = draftLines[line.id_sub_posem];
            var sourceQty = draftLine.Jumlah ?? 0;
            var discount = sourceQty == 0 ? 0 : (draftLine.Diskon ?? 0) * line.Jumlah / sourceQty;
            var total = line.Jumlah * line.Harga - discount;
            var sourceLineBasis = draftLine.Total ??
                ((draftLine.Jumlah ?? 0) * (draftLine.Harga ?? 0) - (draftLine.Diskon ?? 0));
            confirmationBasis += sourceQty == 0 ? 0 : sourceLineBasis * line.Jumlah / sourceQty;
            gross += total;
            draftLine.JumlahKonfirm = (draftLine.JumlahKonfirm ?? 0) + line.Jumlah;

            lines.Add(new SubPO
            {
                Doku = doku,
                Doku_POSem = request.Doku_POSem,
                Kode_Brg = line.Kode_Brg,
                Merk = draftLine.Merk,
                Model = draftLine.Model,
                Satuan = draftLine.Satuan,
                Jumlah = line.Jumlah,
                JumlahKonfirm = line.Jumlah,
                Harga = line.Harga,
                DiscPct = draftLine.DiscPct,
                Diskon = discount,
                Total = total,
                Kode_Gudang = line.Kode_Gudang ?? draftLine.Kode_Gudang,
                Alias = draftLine.Alias,
                Keterangan = line.Note ?? draftLine.Keterangan,
                TglKirim = draftLine.TglKirim,
                Kode_Valas = draftLine.Kode_Valas ?? draft.Kode_Valas,
                PPN = draftLine.PPN,
                Kode_Dept = draft.Kode_dept,
                KodeRnd = draftLine.KodeRnd,
                Tgl = request.Tgl,
                EntryDate = DateTime.UtcNow
            });
        }

        var finalConfirmation = draftLines.Values.All(line =>
            (line.JumlahKonfirm ?? 0) >= (line.Jumlah ?? 0) - 0.0001);
        var allocationRatio = sourceBasis > 0 ? confirmationBasis / sourceBasis : 0;
        var sourceDiskon = draft.Diskon ?? 0;
        var sourceDpp = draft.DPPNilaiLain is > 0 ? draft.DPPNilaiLain.Value : 0;
        var sourcePpnTunai = draft.PPnTunai ?? 0;
        var sourcePpn = draft.PPN ?? 0;
        var hasAbsolutePpn = sourcePpn > 100;
        var sourceAbsolutePpn = hasAbsolutePpn ? sourcePpn : 0;
        var remainingDiskon = Math.Max(0, sourceDiskon - priorConfirmations.Sum(po => po.Diskon ?? 0));
        var remainingDpp = Math.Max(0, sourceDpp - priorConfirmations.Sum(po => po.DPPNilaiLain ?? 0));
        var remainingPpnTunai = Math.Max(0, sourcePpnTunai - priorConfirmations.Sum(po => po.PPnTunai ?? 0));
        var remainingPpn = Math.Max(0, sourceAbsolutePpn - priorConfirmations.Sum(po => hasAbsolutePpn ? po.PPN ?? 0 : 0));
        var allocatedDiskon = finalConfirmation
            ? remainingDiskon
            : Math.Min(remainingDiskon, sourceDiskon * allocationRatio);
        var allocatedDpp = finalConfirmation
            ? remainingDpp
            : Math.Min(remainingDpp, sourceDpp * allocationRatio);
        var allocatedPpnTunai = finalConfirmation
            ? remainingPpnTunai
            : Math.Min(remainingPpnTunai, sourcePpnTunai * allocationRatio);
        var allocatedPpn = finalConfirmation
            ? remainingPpn
            : Math.Min(remainingPpn, sourceAbsolutePpn * allocationRatio);
        var lineNet = gross - allocatedDiskon;
        var dpp = sourceDpp > 0 ? allocatedDpp : lineNet;
        var storedPpn = hasAbsolutePpn ? allocatedPpn : sourcePpn;
        var vat = hasAbsolutePpn ? allocatedPpn : dpp * sourcePpn / 100d;
        _context.POs.Add(new PO
        {
            Doku = doku,
            Doku_POSem = request.Doku_POSem,
            Tgl = request.Tgl,
            Kode_Supplier = draft.Kode_Supplier,
            DokuVendor = draft.DokuVendor,
            TglDokuVendor = draft.TglDokuVendor,
            BLAWB = draft.BLAWB,
            Carrier = draft.Carrier,
            Vessel = draft.Vessel,
            Arrival = draft.Arrival,
            PIUD = draft.PIUD,
            TglPIUD = draft.TglPIUD,
            Ship = draft.Ship,
            TglShip = request.Etd ?? draft.TglShip,
            TglDeparture = draft.TglDeparture,
            Discharge = draft.Discharge,
            Loading = draft.Loading,
            CountryOrigin = draft.CountryOrigin,
            TglCountryOrigin = draft.TglCountryOrigin,
            Weight = draft.Weight,
            Kode_dept = draft.Kode_dept,
            Memo = request.Memo ?? draft.Memo,
            ContactPr = request.ContactPr ?? draft.ContactPr,
            Revisi = draft.Revisi,
            Terms = draft.Terms,
            PPH22 = draft.PPH22,
            Kode_Valas = draft.Kode_Valas,
            Kurs = draft.Kurs,
            Syarat = draft.Syarat,
            STS = "0",
            PPN = storedPpn,
            Diskon = allocatedDiskon,
            DiskonTunai = draft.DiskonTunai,
            PPnBM = draft.PPnBM,
            DPPNilaiLain = sourceDpp > 0 ? allocatedDpp : draft.DPPNilaiLain,
            PPnTunai = allocatedPpnTunai,
            Nilai = dpp + vat + allocatedPpnTunai,
            LC = draft.LC,
            Tgl_Pengiriman = request.Psd ?? draft.Tgl_Pengiriman,
            Tgl_Pembayaran = draft.Tgl_Pembayaran,
            Pembayaran = draft.Pembayaran,
            Penyelesaian = draft.Penyelesaian,
            ADDITIONAL = draft.ADDITIONAL,
            PEMBUATAN = draft.PEMBUATAN,
            Doku_SPPB = draft.Doku_SPPB,
            Jml_Print = draft.Jml_Print,
            DokuExt = draft.DokuExt,
            MOS = draft.MOS,
            Packing = draft.Packing,
            Sign = draft.Sign,
            Tipe = draft.Tipe,
            STSPrint = draft.STSPrint,
            Kode_buyer = draft.Kode_buyer,
            BiayaMasuk = draft.BiayaMasuk,
            BiayaMasukP = draft.BiayaMasukP,
            Kode_IDN = draft.Kode_IDN,
            ModulSource = draft.ModulSource,
            CreatedInWMS = draft.CreatedInWMS,
            CreatedByInWMS = draft.CreatedByInWMS,
            CreatedDateInWMS = draft.CreatedDateInWMS,
            EntryDate = DateTime.UtcNow,
            Wkt = DateTime.UtcNow
        });
        _context.SubPOs.AddRange(lines);

        draft.STS = finalConfirmation ? "1" : "0";

        await _context.SaveChangesAsync(cancellationToken);
        return doku;
    }
}
