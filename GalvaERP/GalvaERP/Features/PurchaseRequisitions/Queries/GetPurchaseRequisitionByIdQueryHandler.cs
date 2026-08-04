using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GalvaERP.Common.Exceptions;
using GalvaERP.Features.PurchaseRequisitions.DTOs;
using GalvaERP.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GalvaERP.Features.PurchaseRequisitions.Queries;

public class GetPurchaseRequisitionByIdQueryHandler : IRequestHandler<GetPurchaseRequisitionByIdQuery, PRDetailDto?>
{
    private readonly AppDbContext _context;

    public GetPurchaseRequisitionByIdQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PRDetailDto?> Handle(GetPurchaseRequisitionByIdQuery request, CancellationToken cancellationToken)
    {
        var pr = await _context.SPBs
            .AsNoTracking()
            .Where(s => s.Doku == request.Doku && s.Hapus == null)
            .Select(s => new
            {
                s.Doku,
                s.Tgl,
                s.Kode_Dept,
                s.Status,
                s.NPO,
                s.Kode_Sales,
                s.Total,
                s.MEMO,
                s.StsVerify,
                s.TglVerify,
                s.RowVersion
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (pr is null)
        {
            throw new NotFoundException($"Purchase Requisition '{request.Doku}' was not found.");
        }

        var lines = await _context.SubSPBs
            .AsNoTracking()
            .Where(sub => sub.Doku == request.Doku && sub.Hapus == null)
            .OrderBy(sub => sub.id_sub_spb)
            .Select(sub => new PRDetailLineDto(
                sub.id_sub_spb,
                sub.Kode_Brg,
                sub.Jumlah,
                sub.Harga,
                sub.Nilai,
                sub.Kode_Gudang,
                sub.Alias))
            .ToListAsync(cancellationToken);

        return new PRDetailDto(
            pr.Doku ?? string.Empty,
            pr.Tgl,
            pr.Kode_Dept,
            pr.Status,
            pr.NPO,
            pr.Kode_Sales,
            pr.Total,
            pr.MEMO,
            pr.StsVerify,
            pr.TglVerify,
            Convert.ToBase64String(pr.RowVersion),
            lines);
    }
}
