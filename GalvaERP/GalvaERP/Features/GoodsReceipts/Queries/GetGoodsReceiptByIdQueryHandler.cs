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
                lpb.Doku_PCF,
                lpb.Kode_Supplier,
                s != null ? s.Nama : null,
                lpb.Kode_Valas,
                lpb.Kurs,
                lpb.SuratJalan,
                lpb.Nilai,
                lpb.PPN,
                lpb.STS,
                lpb.Status,
                lpb.Memo,
                lpb.RowVersion != null ? Convert.ToBase64String(lpb.RowVersion) : string.Empty,
                new List<GRDetailLineDto>())
        ).FirstOrDefaultAsync(cancellationToken);

        if (result is null)
        {
            throw new NotFoundException($"Goods receipt '{request.Doku}' not found");
        }

        var lines = await _context.SubLPBs
            .AsNoTracking()
            .Where(sl => sl.Doku == request.Doku)
            .OrderBy(sl => sl.id_sub_lpb)
            .Select(sl => new GRDetailLineDto(
                sl.id_sub_po_confirmation ?? 0,
                sl.Kode_Brg,
                sl.Jumlah,
                sl.Harga,
                sl.Nilai,
                sl.Kode_Gudang))
            .ToListAsync(cancellationToken);

        return result with { LineItems = lines };
    }
}
