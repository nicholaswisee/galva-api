using GalvaERP.Common.Exceptions;
using GalvaERP.Features.Payments.DTOs;
using GalvaERP.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GalvaERP.Features.Payments.Queries;

public class GetPaymentByIdQueryHandler : IRequestHandler<GetPaymentByIdQuery, PaymentDetailDto>
{
    private readonly AppDbContext _context;

    public GetPaymentByIdQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PaymentDetailDto> Handle(GetPaymentByIdQuery request, CancellationToken cancellationToken)
    {
        var result = await (
            from b in _context.Bayars.AsNoTracking()
            join s in _context.Suppliers.AsNoTracking() on b.Kode_Supplier equals s.Kode into suppliers
            from s in suppliers.DefaultIfEmpty()
            where b.Doku == request.Doku
            select new PaymentDetailDto(
                b.Doku ?? string.Empty,
                b.Tgl,
                b.Kode_Supplier,
                s != null ? s.Nama : null,
                b.Kode_BankSupplier,
                b.Keterangan,
                b.NilaiKas,
                b.NilaiGiro,
                b.NilMuka,
                b.STS,
                b.Kode_Valas,
                b.Kurs,
                b.RowVersion != null ? Convert.ToBase64String(b.RowVersion) : string.Empty)
        ).FirstOrDefaultAsync(cancellationToken);

        if (result is null)
        {
            throw new NotFoundException($"Payment '{request.Doku}' not found");
        }

        return result;
    }
}
