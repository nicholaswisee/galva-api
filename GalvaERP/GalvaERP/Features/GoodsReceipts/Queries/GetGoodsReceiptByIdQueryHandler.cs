using GalvaERP.Common.Exceptions;
using GalvaERP.Features.GoodsReceipts.DTOs;
using GalvaERP.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GalvaERP.Features.GoodsReceipts.Queries;

public class GetGoodsReceiptByIdQueryHandler : IRequestHandler<GetGoodsReceiptByIdQuery, GRDetailDto>
{
    private readonly AppDbContext _context;

    public GetGoodsReceiptByIdQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<GRDetailDto> Handle(GetGoodsReceiptByIdQuery request, CancellationToken cancellationToken)
    {
        var result = await (
            from lpb in _context.LPBs.AsNoTracking()
            join s in _context.Suppliers.AsNoTracking() on lpb.Kode_Supplier equals s.Kode into suppliers
            from s in suppliers.DefaultIfEmpty()
            where lpb.Doku == request.Doku
            select new GRDetailDto(
                lpb.Doku ?? string.Empty,
                lpb.Tgl,
                lpb.Doku_PO,
                lpb.Kode_Supplier,
                s != null ? s.Nama : null,
                lpb.SuratJalan,
                lpb.Nilai,
                lpb.PPN,
                lpb.STS,
                lpb.Status,
                lpb.Memo,
                lpb.RowVersion != null ? Convert.ToBase64String(lpb.RowVersion) : string.Empty)
        ).FirstOrDefaultAsync(cancellationToken);

        if (result is null)
        {
            throw new NotFoundException($"Goods receipt '{request.Doku}' not found");
        }

        return result;
    }
}
