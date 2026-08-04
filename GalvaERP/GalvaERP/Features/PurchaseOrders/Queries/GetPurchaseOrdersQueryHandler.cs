using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GalvaERP.Features.PurchaseOrders.DTOs;
using GalvaERP.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GalvaERP.Features.PurchaseOrders.Queries;

public class GetPurchaseOrdersQueryHandler : IRequestHandler<GetPurchaseOrdersQuery, System.Collections.Generic.List<POListDto>>
{
    private readonly AppDbContext _context;

    public GetPurchaseOrdersQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<System.Collections.Generic.List<POListDto>> Handle(GetPurchaseOrdersQuery request, CancellationToken cancellationToken)
    {
        var query =
            from po in _context.POs.AsNoTracking()
            join s in _context.Suppliers.AsNoTracking() on po.Kode_Supplier equals s.Kode into suppliers
            from s in suppliers.DefaultIfEmpty()
            where po.Doku != null && po.Hapus != "Y"
            orderby po.Tgl descending, po.Doku descending
            select new POListDto
            {
                Doku = po.Doku ?? string.Empty,
                Tgl = po.Tgl,
                Kode_Supplier = po.Kode_Supplier,
                SupplierName = s != null ? s.Nama : null,
                Nilai = po.Nilai,
                STS = po.STS,
                StsVerify = po.StsVerify,
                ETag = System.Convert.ToBase64String(po.RowVersion)
            };

        return await query.ToListAsync(cancellationToken);
    }
}
