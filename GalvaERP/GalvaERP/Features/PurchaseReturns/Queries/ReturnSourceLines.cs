using GalvaERP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GalvaERP.Features.PurchaseReturns.Queries;

internal sealed record ReturnSourceLine(
    string? Doku_LPB,
    string? NPO,
    string? Kode_Brg,
    string? Alias,
    string? Kode_Gudang,
    double Harga,
    double HPP,
    double PPnBm,
    double JumlahTersedia,
    double AlreadyReturned);

internal static class ReturnSourceLines
{
    internal static async Task<List<ReturnSourceLine>> LoadAsync(
        AppDbContext context,
        string dokuFaktur,
        CancellationToken cancellationToken)
    {
        var sourceRows = await (
            from invoice in context.VoucherAPs.AsNoTracking()
            where invoice.Doku == dokuFaktur
            join invoiceLine in context.SubVoucherAPs.AsNoTracking() on invoice.Doku equals invoiceLine.Doku
            where invoiceLine.Doku_LPB != null
            join receiptLine in context.SubLPBs.AsNoTracking() on invoiceLine.Doku_LPB equals receiptLine.Doku
            join item in context.Barangs.AsNoTracking() on receiptLine.Kode_Brg equals item.Kode into items
            from item in items.DefaultIfEmpty()
            select new
            {
                Doku_LPB = receiptLine.Doku,
                NPO = receiptLine.Doku_PO,
                receiptLine.Kode_Brg,
                Alias = item != null ? item.Satuan : null,
                receiptLine.Kode_Gudang,
                Harga = receiptLine.Harga ?? 0.0,
                HPP = receiptLine.Harga ?? 0.0,
                PPnBm = receiptLine.PPnBm ?? 0.0,
                Jumlah = receiptLine.Jumlah ?? 0.0,
            }).ToListAsync(cancellationToken);

        var returnedBySource = await context.SubReturBelis.AsNoTracking()
            .Where(line => line.Doku_Faktur == dokuFaktur && line.Hapus == null)
            .GroupBy(line => new { line.Doku_LPB, line.Kode_Brg, line.Kode_Gudang })
            .Select(group => new
            {
                group.Key.Doku_LPB,
                group.Key.Kode_Brg,
                group.Key.Kode_Gudang,
                Jumlah = group.Sum(line => line.Jumlah ?? 0.0),
            }).ToListAsync(cancellationToken);

        var returned = returnedBySource.ToDictionary(
            line => Key(line.Doku_LPB, line.Kode_Brg, line.Kode_Gudang),
            line => line.Jumlah);

        return sourceRows
            .GroupBy(line => new { line.Doku_LPB, line.Kode_Brg, line.Kode_Gudang })
            .Select(group =>
            {
                var source = group.First();
                var alreadyReturned = returned.GetValueOrDefault(
                    Key(source.Doku_LPB, source.Kode_Brg, source.Kode_Gudang));
                return new ReturnSourceLine(
                    source.Doku_LPB,
                    source.NPO,
                    source.Kode_Brg,
                    source.Alias,
                    source.Kode_Gudang,
                    source.Harga,
                    source.HPP,
                    source.PPnBm,
                    group.Sum(line => line.Jumlah) - alreadyReturned,
                    alreadyReturned);
            })
            .ToList();
    }

    private static string Key(string? dokuLpb, string? kodeBrg, string? kodeGudang) =>
        $"{dokuLpb}\u001F{kodeBrg}\u001F{kodeGudang}";
}
