using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GalvaERP.Common.Exceptions;
using GalvaERP.Features.POConfirmations.DTOs;
using GalvaERP.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GalvaERP.Features.POConfirmations.Queries;

public class GetPOConfirmationByIdQueryHandler : IRequestHandler<GetPOConfirmationByIdQuery, POConfirmationDetailDto?>
{
    private readonly AppDbContext _context;

    public GetPOConfirmationByIdQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<POConfirmationDetailDto?> Handle(GetPOConfirmationByIdQuery request, CancellationToken cancellationToken)
    {
        var query =
            from pc in _context.POConfirmations.AsNoTracking()
            join s in _context.Suppliers.AsNoTracking() on pc.Kode_Supplier equals s.Kode into suppliers
            from s in suppliers.DefaultIfEmpty()
            where pc.Doku == request.Doku
            select new
            {
                pc.Doku,
                pc.Tgl,
                pc.Doku_PO,
                pc.Kode_Supplier,
                SupplierName = s != null ? s.Nama : null,
                pc.Kode_dept,
                pc.Kode_Valas,
                pc.Kurs,
                pc.ContactPr,
                pc.Psd,
                pc.Etd,
                pc.Memo,
                pc.Nilai,
                pc.PPN,
                pc.Diskon,
                pc.STS,
                pc.RowVersion
            };

        var pcRow = await query.FirstOrDefaultAsync(cancellationToken);
        if (pcRow is null)
        {
            throw new NotFoundException($"PO Confirmation '{request.Doku}' was not found.");
        }

        var lines = await _context.SubPOConfirmations
            .AsNoTracking()
            .Where(sc => sc.Doku == request.Doku)
            .OrderBy(sc => sc.id_sub_po_confirmation)
            .Select(sc => new POConfirmationLineDto(
                sc.id_sub_po,
                sc.Kode_Brg,
                sc.Jumlah,
                sc.Harga,
                sc.Total,
                sc.Kode_Gudang,
                sc.Note))
            .ToListAsync(cancellationToken);

        return new POConfirmationDetailDto(
            pcRow.Doku ?? string.Empty,
            pcRow.Tgl,
            pcRow.Doku_PO,
            pcRow.Kode_Supplier,
            pcRow.SupplierName,
            pcRow.Kode_dept,
            pcRow.Kode_Valas,
            pcRow.Kurs,
            pcRow.ContactPr,
            pcRow.Psd,
            pcRow.Etd,
            pcRow.Memo,
            pcRow.Nilai,
            pcRow.PPN,
            pcRow.Diskon,
            pcRow.STS,
            Convert.ToBase64String(pcRow.RowVersion),
            lines);
    }
}
