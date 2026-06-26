using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GalvaERP.Common.Exceptions;
using GalvaERP.Features.PurchaseOrders.DTOs;
using GalvaERP.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GalvaERP.Features.PurchaseOrders.Commands;

public class UpdatePurchaseOrderCommandHandler : IRequestHandler<UpdatePurchaseOrderCommand, PODetailDto>
{
    private readonly AppDbContext _context;

    public UpdatePurchaseOrderCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PODetailDto> Handle(UpdatePurchaseOrderCommand request, CancellationToken cancellationToken)
    {
        if (request.STS is not null && request.STS is not ("0" or "1" or "2"))
        {
            throw new DomainException(
                "STS must be '0' (Pending), '1' (Confirmed), or '2' (Cancelled).");
        }

        var po = await _context.POs
            .FirstOrDefaultAsync(p => p.Doku == request.Doku, cancellationToken);

        if (po is null)
        {
            throw new NotFoundException($"Purchase Order '{request.Doku}' was not found.");
        }

        if (!po.RowVersion.SequenceEqual(request.IfMatchRowVersion))
        {
            throw new ConcurrencyException(
                $"Purchase Order '{request.Doku}' was modified by another user. Please reload and try again.");
        }

        if (request.STS is not null)
        {
            po.STS = request.STS;
        }
        _context.Entry(po).Property(p => p.RowVersion).OriginalValue = request.IfMatchRowVersion;

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConcurrencyException(
                $"Purchase Order '{request.Doku}' was modified by another user. Please reload and try again.");
        }

        var query =
            from fresh in _context.POs.AsNoTracking()
            join s in _context.Suppliers.AsNoTracking() on fresh.Kode_Supplier equals s.Kode into suppliers
            from s in suppliers.DefaultIfEmpty()
            where fresh.Doku == request.Doku
            select new
            {
                fresh.Doku,
                fresh.Tgl,
                fresh.Kode_Supplier,
                SupplierName = s != null ? s.Nama : null,
                fresh.Kode_dept,
                fresh.Nilai,
                fresh.PPN,
                fresh.Diskon,
                fresh.STS,
                fresh.Memo,
                fresh.RowVersion
            };

        var refreshed = await query.FirstAsync(cancellationToken);

        return new PODetailDto(
            refreshed.Doku ?? string.Empty,
            refreshed.Tgl,
            refreshed.Kode_Supplier,
            refreshed.SupplierName,
            refreshed.Kode_dept,
            refreshed.Nilai,
            refreshed.PPN,
            refreshed.Diskon,
            refreshed.STS,
            refreshed.Memo,
            Convert.ToBase64String(refreshed.RowVersion));
    }
}
