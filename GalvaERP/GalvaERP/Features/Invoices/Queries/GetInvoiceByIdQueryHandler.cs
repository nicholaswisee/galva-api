using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
            select new
            {
                Doku = vap.Doku ?? string.Empty,
                vap.TglDoku,
                vap.Kode_Supplier,
                SupplierName = s != null ? s.Nama : null,
                vap.Kode_Dept,
                vap.Nilai,
                vap.PPn,
                vap.Diskon,
                vap.Misc,
                vap.STS,
                vap.Keterangan,
                vap.TipeBiaya,
                RowVersion = vap.RowVersion != null ? Convert.ToBase64String(vap.RowVersion) : string.Empty
            }
        ).FirstOrDefaultAsync(cancellationToken);

        if (result is null)
        {
            throw new NotFoundException($"AP invoice '{request.Doku}' not found");
        }

        var lines = await _context.SubVoucherAPs
            .AsNoTracking()
            .Where(sub => sub.Doku == request.Doku)
            .OrderBy(sub => sub.PKbas)
            .Select(sub => new InvoiceDetailLineDto(
                sub.PKbas,
                sub.TipeBiaya,
                sub.Doku_LPB,
                sub.Doku_PO,
                sub.NilaiLPB,
                sub.Nilai,
                sub.PPn,
                sub.APRef,
                sub.InvoiceNo,
                sub.TglInvoice,
                sub.Doku_FP))
            .ToListAsync(cancellationToken);

        return new InvoiceDetailDto(
            result.Doku,
            result.TglDoku,
            result.Kode_Supplier,
            result.SupplierName,
            result.Kode_Dept,
            result.Nilai,
            result.PPn,
            result.Diskon,
            result.Misc,
            result.STS,
            result.Keterangan,
            result.TipeBiaya,
            result.RowVersion,
            lines);
    }
}
