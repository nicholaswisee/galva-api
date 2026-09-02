using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GalvaERP.Common.Exceptions;
using GalvaERP.Features.PurchaseOrders.DTOs;
using GalvaERP.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GalvaERP.Features.PurchaseOrders.Queries;

public class GetPurchaseOrderByIdQueryHandler : IRequestHandler<GetPurchaseOrderByIdQuery, PODetailDto?>
{
    private readonly AppDbContext _context;

    public GetPurchaseOrderByIdQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PODetailDto?> Handle(GetPurchaseOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var query =
            from po in _context.POSems.AsNoTracking()
            join s in _context.Suppliers.AsNoTracking() on po.Kode_Supplier equals s.Kode into suppliers
            from s in suppliers.DefaultIfEmpty()
            where po.Doku == request.Doku && po.Hapus != "Y"
            select new
            {
                po.Doku,
                po.Tgl,
                po.Kode_Supplier,
                SupplierName = s != null ? s.Nama : null,
                po.Kode_dept,
                po.Kode_Valas,
                po.Kurs,
                po.Nilai,
                po.DPPNilaiLain,
                po.PPN,
                po.PPnTunai,
                po.Diskon,
                po.Syarat,
                po.STS,
                po.StsVerify,
                po.TglVerify,
                po.Memo,
                po.RowVersion
            };

        var poRow = await query.FirstOrDefaultAsync(cancellationToken);
        if (poRow is null)
        {
            throw new NotFoundException($"Purchase Order '{request.Doku}' was not found.");
        }

        var lines = await _context.SubPOSems
            .AsNoTracking()
            .Where(sp => sp.Doku == request.Doku)
            .OrderBy(sp => sp.id_sub_posem)
            .Select(sp => new PODetailLineDto(
                sp.id_sub_posem,
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
            poRow.Doku ?? string.Empty,
            poRow.Tgl,
            poRow.Kode_Supplier,
            poRow.SupplierName,
            poRow.Kode_dept,
            poRow.Kode_Valas,
            poRow.Kurs,
            poRow.Nilai,
            poRow.DPPNilaiLain,
            poRow.PPN,
            poRow.PPnTunai,
            poRow.Diskon,
            poRow.Syarat,
            poRow.STS,
            poRow.Memo,
            poRow.StsVerify,
            poRow.TglVerify,
            Convert.ToBase64String(poRow.RowVersion),
            lines);
    }
}
