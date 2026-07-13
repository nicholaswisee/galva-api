using GalvaERP.Features.Invoices.DTOs;
using GalvaERP.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GalvaERP.Features.Invoices.Queries;

public class GetInvoicesQueryHandler : IRequestHandler<GetInvoicesQuery, List<InvoiceListDto>>
{
    private readonly AppDbContext _context;

    public GetInvoicesQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<InvoiceListDto>> Handle(GetInvoicesQuery request, CancellationToken cancellationToken)
    {
            var query = from vap in _context.VoucherAPs.AsNoTracking()
                        join s in _context.Suppliers.AsNoTracking() on vap.Kode_Supplier equals s.Kode into suppliers
                        from s in suppliers.DefaultIfEmpty()
                        orderby vap.TglDoku descending
                        select new { vap, s };

            if (!string.IsNullOrWhiteSpace(request.Source))
            {
                query = query.Where(x => x.vap.SourceType == request.Source).OrderByDescending(x => x.vap.TglDoku);
            }

            var result = await query
                .Select(x => new InvoiceListDto
                {
                    Doku = x.vap.Doku ?? string.Empty,
                    Tgl = x.vap.TglDoku,
                    Kode_Supplier = x.vap.Kode_Supplier,
                    SupplierName = x.s != null ? x.s.Nama : null,
                    Nilai = x.vap.Nilai,
                    STS = x.vap.STS,
                    SourceType = x.vap.SourceType,
                    ETag = x.vap.RowVersion != null ? Convert.ToBase64String(x.vap.RowVersion) : null,
                })
                .ToListAsync(cancellationToken);

        return result;
    }
}
