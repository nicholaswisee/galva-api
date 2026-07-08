using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GalvaERP.Features.POConfirmations.DTOs;
using GalvaERP.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GalvaERP.Features.POConfirmations.Queries;

public class GetPOConfirmationsQueryHandler : IRequestHandler<GetPOConfirmationsQuery, List<POConfirmationListDto>>
{
    private readonly AppDbContext _context;

    public GetPOConfirmationsQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<POConfirmationListDto>> Handle(GetPOConfirmationsQuery request, CancellationToken cancellationToken)
    {
        var query =
            from pc in _context.POConfirmations.AsNoTracking()
            join s in _context.Suppliers.AsNoTracking() on pc.Kode_Supplier equals s.Kode into suppliers
            from s in suppliers.DefaultIfEmpty()
            where pc.Doku != null && pc.STS != "9"
            orderby pc.Tgl descending, pc.Doku descending
            select new POConfirmationListDto
            {
                Doku = pc.Doku ?? string.Empty,
                Tgl = pc.Tgl,
                Doku_PO = pc.Doku_PO,
                Kode_Supplier = pc.Kode_Supplier,
                SupplierName = s != null ? s.Nama : null,
                Nilai = pc.Nilai,
                STS = pc.STS,
                ETag = System.Convert.ToBase64String(pc.RowVersion)
            };

        return await query.ToListAsync(cancellationToken);
    }
}
