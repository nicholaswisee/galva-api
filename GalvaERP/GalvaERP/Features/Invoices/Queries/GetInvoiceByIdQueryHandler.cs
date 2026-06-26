using GalvaERP.Common.Exceptions;
using GalvaERP.Features.Invoices.DTOs;
using GalvaERP.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GalvaERP.Features.Invoices.Queries;

public class GetInvoiceByIdQueryHandler : IRequestHandler<GetInvoiceByIdQuery, InvoiceDetailDto>
{
    private readonly AppDbContext _context;

    public GetInvoiceByIdQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<InvoiceDetailDto> Handle(GetInvoiceByIdQuery request, CancellationToken cancellationToken)
    {
        var result = await (
            from vap in _context.VoucherAPs.AsNoTracking()
            join s in _context.Suppliers.AsNoTracking() on vap.Kode_Supplier equals s.Kode into suppliers
            from s in suppliers.DefaultIfEmpty()
            where vap.Doku == request.Doku
            select new InvoiceDetailDto(
                vap.Doku ?? string.Empty,
                vap.Tgl,
                vap.Kode_Supplier,
                s != null ? s.Nama : null,
                vap.Kode_Dept,
                vap.Kode_Bank,
                vap.Nilai,
                vap.PPn,
                vap.Diskon,
                vap.Misc,
                vap.STS,
                vap.Status,
                vap.Keterangan,
                vap.RowVersion != null ? Convert.ToBase64String(vap.RowVersion) : string.Empty)
        ).FirstOrDefaultAsync(cancellationToken);

        if (result is null)
        {
            throw new NotFoundException($"AP invoice '{request.Doku}' not found");
        }

        return result;
    }
}
