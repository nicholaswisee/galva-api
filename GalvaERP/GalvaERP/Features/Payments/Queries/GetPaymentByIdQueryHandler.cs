using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
            where b.Hapus == null && b.Doku == request.Doku
            select new
            {
                Doku = b.Doku ?? string.Empty,
                b.Tgl,
                b.Kode_Supplier,
                SupplierName = s != null ? s.Nama : null,
                b.Kode_BankSupplier,
                b.Keterangan,
                b.NilaiKas,
                b.NilaiGiro,
                b.NilMuka,
                b.STS,
                b.Kode_Valas,
                b.Kurs,
                RowVersion = b.RowVersion != null ? Convert.ToBase64String(b.RowVersion) : string.Empty
            }
        ).FirstOrDefaultAsync(cancellationToken);

        if (result is null)
        {
            throw new NotFoundException($"Payment '{request.Doku}' not found");
        }

        var lines = await _context.SubBayars
            .AsNoTracking()
            .Where(sub => sub.Doku == request.Doku)
            .OrderBy(sub => sub.PKbas)
            .Select(sub => new PaymentDetailLineDto(
                sub.PKbas,
                sub.Doku_Faktur,
                sub.Doku_LPB,
                sub.Nilai,
                sub.TotalNilai,
                sub.DiskonTunai,
                sub.Keterangan))
            .ToListAsync(cancellationToken);

        return new PaymentDetailDto(
            result.Doku,
            result.Tgl,
            result.Kode_Supplier,
            result.SupplierName,
            result.Kode_BankSupplier,
            result.Keterangan,
            result.NilaiKas,
            result.NilaiGiro,
            result.NilMuka,
            result.STS,
            result.Kode_Valas,
            result.Kurs,
            result.RowVersion,
            lines);
    }
}
