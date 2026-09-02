using GalvaERP.Features.PurchaseOrders.DTOs;
using GalvaERP.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GalvaERP.Features.PurchaseOrders.Queries;

public class GetConfirmedPurchaseOrdersQueryHandler : IRequestHandler<GetConfirmedPurchaseOrdersQuery, List<ConfirmedPurchaseOrderListDto>>
{
    private readonly AppDbContext _context;

    public GetConfirmedPurchaseOrdersQueryHandler(AppDbContext context) => _context = context;

    public Task<List<ConfirmedPurchaseOrderListDto>> Handle(
        GetConfirmedPurchaseOrdersQuery request,
        CancellationToken cancellationToken) =>
        (
            from po in _context.POs.AsNoTracking()
            join supplier in _context.Suppliers.AsNoTracking() on po.Kode_Supplier equals supplier.Kode into suppliers
            from supplier in suppliers.DefaultIfEmpty()
            where po.Doku != null && po.Doku_POSem != null && po.Hapus != "Y"
            orderby po.Tgl descending, po.Doku descending
            select new ConfirmedPurchaseOrderListDto
            {
                Doku = po.Doku ?? string.Empty,
                Tgl = po.Tgl,
                Doku_PO = po.Doku ?? string.Empty,
                Doku_POSem = po.Doku_POSem,
                Kode_Supplier = po.Kode_Supplier,
                SupplierName = supplier != null ? supplier.Nama : null,
                Nilai = po.Nilai,
                STS = po.STS,
                ETag = Convert.ToBase64String(po.RowVersion)
            }
        ).ToListAsync(cancellationToken);
}
