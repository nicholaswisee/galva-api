using GalvaERP.Features.PurchaseReturns.DTOs;
using GalvaERP.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GalvaERP.Features.PurchaseReturns.Queries;

public sealed class GetReturnsQueryHandler : IRequestHandler<GetReturnsQuery, List<ReturnListDto>>
{
    private readonly AppDbContext _context;

    public GetReturnsQueryHandler(AppDbContext context) => _context = context;

    public Task<List<ReturnListDto>> Handle(GetReturnsQuery request, CancellationToken cancellationToken) =>
        (from retur in _context.ReturBelis.AsNoTracking()
         where retur.Hapus == null
         join supplier in _context.Suppliers.AsNoTracking() on retur.Kode_Supplier equals supplier.Kode into suppliers
         from supplier in suppliers.DefaultIfEmpty()
         orderby retur.Tgl descending
         select new ReturnListDto
         {
             Doku = retur.Doku ?? string.Empty,
             Tgl = retur.Tgl,
             Doku_Faktur = retur.Doku_Faktur,
             Kode_Supplier = retur.Kode_Supplier,
             SupplierName = supplier != null ? supplier.Nama : null,
             Kode_Valas = retur.Kode_Valas,
             Nilai = retur.NILAI ?? 0.0,
             STS = retur.STS ?? string.Empty,
             SyncToCMG = retur.SyncToCMG ?? false,
             ETag = retur.RowVersion != null ? Convert.ToBase64String(retur.RowVersion) : string.Empty,
         }).ToListAsync(cancellationToken);
}
