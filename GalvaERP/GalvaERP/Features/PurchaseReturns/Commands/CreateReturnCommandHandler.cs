using System.Globalization;
using GalvaERP.Common.Exceptions;
using GalvaERP.Domain.Entities;
using GalvaERP.Features.PurchaseReturns.Queries;
using GalvaERP.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GalvaERP.Features.PurchaseReturns.Commands;

public sealed class CreateReturnCommandHandler : IRequestHandler<CreateReturnCommand, string>
{
    private readonly AppDbContext _context;

    public CreateReturnCommandHandler(AppDbContext context) => _context = context;

    public async Task<string> Handle(CreateReturnCommand request, CancellationToken cancellationToken)
    {
        var invoice = await _context.VoucherAPs
            .FirstOrDefaultAsync(source => source.Doku == request.Doku_Faktur, cancellationToken);
        if (invoice is null)
            throw new NotFoundException($"Source invoice {request.Doku_Faktur} does not exist");

        var duplicate = request.LineItems
            .GroupBy(line => new { line.Doku_LPB, line.Kode_Brg, line.Kode_Gudang })
            .FirstOrDefault(group => group.Count() > 1);
        if (duplicate is not null)
        {
            var line = duplicate.First();
            throw new DomainException(
                $"Duplicate return line for item {line.Kode_Brg}. Source={line.Doku_LPB}, warehouse={line.Kode_Gudang}.");
        }

        var sources = await ReturnSourceLines.LoadAsync(_context, request.Doku_Faktur, cancellationToken);
        var resolvedLines = new List<(CreateReturnLineItem Request, ReturnSourceLine Source)>();
        foreach (var line in request.LineItems)
        {
            var source = sources.SingleOrDefault(candidate =>
                string.Equals(line.Doku_Faktur, request.Doku_Faktur, StringComparison.Ordinal) &&
                string.Equals(candidate.Doku_LPB, line.Doku_LPB, StringComparison.Ordinal) &&
                string.Equals(candidate.Kode_Brg, line.Kode_Brg, StringComparison.Ordinal) &&
                string.Equals(candidate.Kode_Gudang, line.Kode_Gudang, StringComparison.Ordinal));
            if (source is null)
            {
                throw new DomainException(
                    $"Return line for item {line.Kode_Brg} does not belong to source invoice {request.Doku_Faktur}.");
            }

            if (line.Jumlah > source.JumlahTersedia + 0.0001)
            {
                throw new DomainException(
                    $"Returned quantity for item {line.Kode_Brg} exceeds the remaining source quantity. " +
                    $"Source={line.Doku_LPB}, already returned={FormatQuantity(source.AlreadyReturned)}, " +
                    $"remaining={FormatQuantity(source.JumlahTersedia)}, requested={FormatQuantity(line.Jumlah)}.");
            }

            resolvedLines.Add((line, source));
        }

        var prefix = $"RET-{request.Tgl:yyyyMMdd}-";
        var doku = prefix + ((await _context.ReturBelis
            .CountAsync(retur => retur.Doku != null && retur.Doku.StartsWith(prefix), cancellationToken)) + 1).ToString("D3");

        var total = resolvedLines.Sum(line => line.Request.Jumlah * line.Source.Harga);
        var diskon = resolvedLines.Sum(line => line.Request.Diskon);
        var net = total - diskon;
        var nilai = net + (net * request.PPn / 100.0);
        var warehouses = resolvedLines.Select(line => line.Source.Kode_Gudang).Distinct().ToList();

        var retur = new ReturBeli
        {
            Doku = doku,
            Tgl = request.Tgl,
            Doku_Faktur = request.Doku_Faktur,
            Kode_Supplier = invoice.Kode_Supplier,
            Kode_Dept = request.Kode_Dept,
            Kode_Gudang = warehouses.Count == 1 ? warehouses[0] : null,
            PPn = request.PPn,
            Diskon = diskon,
            Total = total,
            Kode_Valas = request.Kode_Valas,
            Kurs = request.Kurs,
            STS = "0",
            NILAI = nilai,
            Type = request.Type,
            TipeRetur = request.TipeRetur,
            Doku_FP = request.Doku_FP,
            Tgl_FP = request.Tgl_FP,
            MEMO = request.Memo,
            Validasi = false,
            SyncToCMG = false,
            CreatedInWMS = false,
            EntryDate = DateTime.UtcNow,
        };

        var lines = resolvedLines.Select(line => new SubReturBeli
        {
            Doku = doku,
            Tgl = request.Tgl,
            Kode_Supplier = invoice.Kode_Supplier,
            Doku_Faktur2 = request.Doku_Faktur,
            NPO = line.Source.NPO,
            Kode_Dept = request.Kode_Dept,
            Kode_Brg = line.Source.Kode_Brg,
            Kode_Gudang = line.Source.Kode_Gudang,
            Alias = line.Source.Alias,
            Jumlah = line.Request.Jumlah,
            Harga = line.Source.Harga,
            HPP = line.Source.HPP,
            Diskon = line.Request.Diskon,
            PPN = (line.Request.Jumlah * line.Source.Harga - line.Request.Diskon) * request.PPn / 100.0,
            PPnBm = line.Source.PPnBm,
            Nilai = line.Request.Jumlah * line.Source.Harga - line.Request.Diskon,
            Kode_Valas = request.Kode_Valas,
            Kurs = request.Kurs,
            NoUrut = line.Request.NoUrut,
            EntryDate = DateTime.UtcNow,
            Doku_Faktur = request.Doku_Faktur,
            Doku_LPB = line.Source.Doku_LPB,
        }).ToList();

        _context.ReturBelis.Add(retur);
        _context.SubReturBelis.AddRange(lines);
        await _context.SaveChangesAsync(cancellationToken);
        return doku;
    }

    private static string FormatQuantity(double value) => value.ToString("0.####", CultureInfo.InvariantCulture);
}
