using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GalvaERP.Common.Exceptions;
using GalvaERP.Features.PurchaseRequisitions.DTOs;
using GalvaERP.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GalvaERP.Features.PurchaseRequisitions.Commands;

public class UpdatePurchaseRequisitionCommandHandler : IRequestHandler<UpdatePurchaseRequisitionCommand, PRDetailDto>
{
    private readonly AppDbContext _context;

    public UpdatePurchaseRequisitionCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PRDetailDto> Handle(UpdatePurchaseRequisitionCommand request, CancellationToken cancellationToken)
    {
        var spb = await _context.SPBs
            .FirstOrDefaultAsync(s => s.Doku == request.Doku, cancellationToken);

        if (spb is null)
        {
            throw new NotFoundException($"Purchase Requisition '{request.Doku}' was not found.");
        }

        if (!spb.RowVersion.SequenceEqual(request.IfMatchRowVersion))
        {
            throw new ConcurrencyException(
                $"Purchase Requisition '{request.Doku}' was modified by another user. Please reload and try again.");
        }

        spb.Status = request.Status;
        _context.Entry(spb).Property(s => s.RowVersion).OriginalValue = request.IfMatchRowVersion;

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConcurrencyException(
                $"Purchase Requisition '{request.Doku}' was modified by another user. Please reload and try again.");
        }

        var fresh = await _context.SPBs
            .AsNoTracking()
            .FirstAsync(s => s.Doku == request.Doku, cancellationToken);

        return new PRDetailDto(
            fresh.Doku ?? string.Empty,
            fresh.Tgl,
            fresh.Kode_Dept,
            fresh.Status,
            fresh.NPO,
            fresh.Kode_Sales,
            fresh.Total,
            fresh.MEMO,
            Convert.ToBase64String(fresh.RowVersion));
    }
}
