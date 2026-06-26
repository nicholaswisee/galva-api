using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GalvaERP.Common.Exceptions;
using GalvaERP.Features.PurchaseOrders.DTOs;
using GalvaERP.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GalvaERP.Features.PurchaseOrders.Queries;

public class GetPurchaseOrderByIdQueryHandler : IRequestHandler<GetPurchaseOrderByIdQuery, PODetailDto?>
{
    private readonly AppDbContext _context;

    public GetPurchaseOrderByIdQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PODetailDto?> Handle(GetPurchaseOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var query =
            from po in _context.POs.AsNoTracking()
            join s in _context.Suppliers.AsNoTracking() on po.Kode_Supplier equals s.Kode into suppliers
            from s in suppliers.DefaultIfEmpty()
            where po.Doku == request.Doku
            select new
            {
                po.Doku,
                po.Tgl,
                po.Kode_Supplier,
                SupplierName = s != null ? s.Nama : null,
                po.Kode_dept,
                po.Nilai,
                po.PPN,
                po.Diskon,
                po.STS,
                po.Memo,
                po.RowVersion
            };

        var poRow = await query.FirstOrDefaultAsync(cancellationToken);
        if (poRow is null)
        {
            throw new NotFoundException($"Purchase Order '{request.Doku}' was not found.");
        }

        return new PODetailDto(
            poRow.Doku ?? string.Empty,
            poRow.Tgl,
            poRow.Kode_Supplier,
            poRow.SupplierName,
            poRow.Kode_dept,
            poRow.Nilai,
            poRow.PPN,
            poRow.Diskon,
            poRow.STS,
            poRow.Memo,
            Convert.ToBase64String(poRow.RowVersion));
    }
}
