using GalvaERP.Features.GoodsReceipts.DTOs;
using GalvaERP.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GalvaERP.Features.GoodsReceipts.Queries;

public class GetGoodsReceiptsQueryHandler : IRequestHandler<GetGoodsReceiptsQuery, List<GRListDto>>
{
    private readonly AppDbContext _context;

    public GetGoodsReceiptsQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<GRListDto>> Handle(GetGoodsReceiptsQuery request, CancellationToken cancellationToken)
    {
        var result = await (
            from lpb in _context.LPBs.AsNoTracking()
            join s in _context.Suppliers.AsNoTracking() on lpb.Kode_Supplier equals s.Kode into suppliers
            from s in suppliers.DefaultIfEmpty()
            orderby lpb.Tgl descending
            select new GRListDto
            {
                Doku = lpb.Doku ?? string.Empty,
                Tgl = lpb.Tgl,
                Doku_PO = lpb.Doku_PO,
                Doku_PCF = lpb.Doku_PCF,
                Kode_Supplier = lpb.Kode_Supplier,
                SupplierName = s != null ? s.Nama : null,
                Nilai = lpb.Nilai,
                STS = lpb.STS,
                Status = lpb.Status,
                ETag = lpb.RowVersion != null ? Convert.ToBase64String(lpb.RowVersion) : null,
            }
        ).ToListAsync(cancellationToken);

        return result;
    }
}
