using GalvaERP.Features.Payments.DTOs;
using GalvaERP.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GalvaERP.Features.Payments.Queries;

public class GetPaymentsQueryHandler : IRequestHandler<GetPaymentsQuery, List<PaymentListDto>>
{
    private readonly AppDbContext _context;

    public GetPaymentsQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<PaymentListDto>> Handle(GetPaymentsQuery request, CancellationToken cancellationToken)
    {
        var result = await (
            from b in _context.Bayars.AsNoTracking()
            where b.Hapus == null
            join s in _context.Suppliers.AsNoTracking() on b.Kode_Supplier equals s.Kode into suppliers
            from s in suppliers.DefaultIfEmpty()
            orderby b.Tgl descending
            select new PaymentListDto
            {
                Doku = b.Doku ?? string.Empty,
                Tgl = b.Tgl,
                Kode_Supplier = b.Kode_Supplier,
                SupplierName = s != null ? s.Nama : null,
                NilaiKas = b.NilaiKas,
                NilaiGiro = b.NilaiGiro,
                STS = b.STS,
                ETag = b.RowVersion != null ? Convert.ToBase64String(b.RowVersion) : null,
            }
        ).ToListAsync(cancellationToken);

        return result;
    }
}
