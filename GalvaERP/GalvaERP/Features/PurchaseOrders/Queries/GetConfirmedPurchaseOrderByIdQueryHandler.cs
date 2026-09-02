using GalvaERP.Common.Exceptions;
using GalvaERP.Features.PurchaseOrders.DTOs;
using GalvaERP.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GalvaERP.Features.PurchaseOrders.Queries;

public class GetConfirmedPurchaseOrderByIdQueryHandler : IRequestHandler<GetConfirmedPurchaseOrderByIdQuery, ConfirmedPurchaseOrderDetailDto?>
{
    private readonly AppDbContext _context;

    public GetConfirmedPurchaseOrderByIdQueryHandler(AppDbContext context) => _context = context;

    public async Task<ConfirmedPurchaseOrderDetailDto?> Handle(
        GetConfirmedPurchaseOrderByIdQuery request,
        CancellationToken cancellationToken)
    {
        var row = await (
            from po in _context.POs.AsNoTracking()
            join supplier in _context.Suppliers.AsNoTracking() on po.Kode_Supplier equals supplier.Kode into suppliers
            from supplier in suppliers.DefaultIfEmpty()
            where po.Doku == request.Doku && po.Doku_POSem != null && po.Hapus != "Y"
            select new
            {
                po.Doku,
                po.Tgl,
                po.Doku_POSem,
                po.Kode_Supplier,
                SupplierName = supplier != null ? supplier.Nama : null,
                po.Kode_dept,
                po.Kode_Valas,
                po.Kurs,
                po.ContactPr,
                po.Tgl_Pengiriman,
                po.TglShip,
                po.Memo,
                po.Nilai,
                po.PPN,
                po.Diskon,
                po.STS,
                po.RowVersion
            }
        ).FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException($"Confirmed purchase order '{request.Doku}' was not found.");

        var lines = await _context.SubPOs
            .AsNoTracking()
            .Where(line => line.Doku == request.Doku)
            .OrderBy(line => line.id_sub_po)
            .Select(line => new ConfirmedPurchaseOrderLineDto(
                line.id_sub_po,
                line.id_sub_po,
                line.Kode_Brg,
                line.Jumlah,
                line.Harga,
                line.Total,
                line.Kode_Gudang,
                line.Keterangan))
            .ToListAsync(cancellationToken);

        return new ConfirmedPurchaseOrderDetailDto(
            row.Doku ?? string.Empty,
            row.Tgl,
            row.Doku,
            row.Doku_POSem,
            row.Kode_Supplier,
            row.SupplierName,
            row.Kode_dept,
            row.Kode_Valas,
            row.Kurs,
            row.ContactPr,
            row.Tgl_Pengiriman,
            row.TglShip,
            row.Memo,
            row.Nilai,
            row.PPN,
            row.Diskon,
            row.STS,
            Convert.ToBase64String(row.RowVersion),
            lines);
    }
}
