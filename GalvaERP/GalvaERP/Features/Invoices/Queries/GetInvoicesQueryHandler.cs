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
        var result = await (
            from vap in _context.VoucherAPs.AsNoTracking()
            join s in _context.Suppliers.AsNoTracking() on vap.Kode_Supplier equals s.Kode into suppliers
            from s in suppliers.DefaultIfEmpty()
            orderby vap.TglDoku descending
            select new InvoiceListDto
            {
                Doku = vap.Doku ?? string.Empty,
                Tgl = vap.TglDoku,
                Kode_Supplier = vap.Kode_Supplier,
                SupplierName = s != null ? s.Nama : null,
                Nilai = vap.Nilai,
                STS = vap.STS,
                ETag = vap.RowVersion != null ? Convert.ToBase64String(vap.RowVersion) : null,
            }
        ).ToListAsync(cancellationToken);

        return result;
    }
}
