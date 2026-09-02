using GalvaERP.Common.Exceptions;
using GalvaERP.Features.PurchaseReturns.DTOs;
using GalvaERP.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GalvaERP.Features.PurchaseReturns.Queries;

public sealed class GetReturnByIdQueryHandler : IRequestHandler<GetReturnByIdQuery, ReturnDetailDto>
{
    private readonly AppDbContext _context;

    public GetReturnByIdQueryHandler(AppDbContext context) => _context = context;

    public async Task<ReturnDetailDto> Handle(GetReturnByIdQuery request, CancellationToken cancellationToken)
    {
        var header = await (
            from retur in _context.ReturBelis.AsNoTracking()
            where retur.Doku == request.Doku && retur.Hapus == null
            join supplier in _context.Suppliers.AsNoTracking() on retur.Kode_Supplier equals supplier.Kode into suppliers
            from supplier in suppliers.DefaultIfEmpty()
            select new
            {
                Doku = retur.Doku ?? string.Empty,
                retur.Tgl,
                retur.Doku_Faktur,
                retur.Kode_Supplier,
                SupplierName = supplier != null ? supplier.Nama : null,
                retur.Kode_Dept,
                retur.Kode_Valas,
                retur.Kurs,
                retur.PPn,
                retur.Diskon,
                retur.Total,
                retur.NILAI,
                retur.MEMO,
                retur.STS,
                retur.StatusGL,
                retur.Validasi,
                retur.SyncToCMG,
                retur.CreatedInWMS,
                retur.Type,
                retur.TipeRetur,
                retur.Doku_FP,
                retur.Tgl_FP,
                ETag = retur.RowVersion != null ? Convert.ToBase64String(retur.RowVersion) : string.Empty,
            }).FirstOrDefaultAsync(cancellationToken);

        if (header is null)
            throw new NotFoundException($"Purchase return '{request.Doku}' not found.");

        var lines = await _context.SubReturBelis.AsNoTracking()
            .Where(line => line.Doku == request.Doku && line.Hapus == null)
            .OrderBy(line => line.NoUrut)
            .ThenBy(line => line.PKbas)
            .Select(line => new ReturnDetailLineDto(
                line.PKbas,
                line.Doku_Faktur,
                line.Doku_LPB,
                line.NPO,
                line.Kode_Brg,
                line.Kode_Gudang,
                line.Alias,
                line.Jumlah ?? 0.0,
                line.Harga ?? 0.0,
                line.Diskon ?? 0.0,
                line.PPN ?? 0.0,
                line.PPnBm ?? 0.0,
                line.HPP ?? 0.0,
                line.Nilai ?? 0.0,
                line.NoUrut ?? 0))
            .ToListAsync(cancellationToken);

        return new ReturnDetailDto(
            header.Doku,
            header.Tgl,
            header.Doku_Faktur,
            header.Kode_Supplier,
            header.SupplierName,
            header.Kode_Dept,
            header.Kode_Valas,
            header.Kurs ?? 0.0,
            header.PPn ?? 0.0,
            header.Diskon ?? 0.0,
            header.Total ?? 0.0,
            header.NILAI ?? 0.0,
            header.MEMO,
            header.STS ?? string.Empty,
            header.StatusGL,
            header.Validasi ?? false,
            header.SyncToCMG ?? false,
            header.CreatedInWMS ?? false,
            header.Type,
            header.TipeRetur,
            header.Doku_FP,
            header.Tgl_FP,
            header.ETag,
            lines);
    }
}
