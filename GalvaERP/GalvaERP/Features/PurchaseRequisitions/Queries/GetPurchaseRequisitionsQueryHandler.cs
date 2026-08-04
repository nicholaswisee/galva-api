using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GalvaERP.Features.PurchaseRequisitions.DTOs;
using GalvaERP.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GalvaERP.Features.PurchaseRequisitions.Queries;

public class GetPurchaseRequisitionsQueryHandler : IRequestHandler<GetPurchaseRequisitionsQuery, List<PRListDto>>
{
    private readonly AppDbContext _context;

    public GetPurchaseRequisitionsQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<PRListDto>> Handle(GetPurchaseRequisitionsQuery request, CancellationToken cancellationToken)
    {
        return await _context.SPBs
            .AsNoTracking()
            .Where(s => s.Doku != null && s.Hapus == null)
            .OrderByDescending(s => s.Tgl)
            .ThenByDescending(s => s.Doku)
            .Select(s => new PRListDto
            {
                Doku = s.Doku ?? string.Empty,
                Tgl = s.Tgl ?? DateTime.MinValue,
                Kode_Dept = s.Kode_Dept,
                Status = s.Status,
                StsVerify = s.StsVerify,
                ETag = Convert.ToBase64String(s.RowVersion)
            })
            .ToListAsync(cancellationToken);
    }
}
