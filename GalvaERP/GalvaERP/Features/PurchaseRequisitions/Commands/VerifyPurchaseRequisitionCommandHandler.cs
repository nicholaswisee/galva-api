using GalvaERP.Common.Exceptions;
using GalvaERP.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GalvaERP.Features.PurchaseRequisitions.Commands;

public class VerifyPurchaseRequisitionCommandHandler : IRequestHandler<VerifyPurchaseRequisitionCommand, Unit>
{
    private readonly AppDbContext _context;

    public VerifyPurchaseRequisitionCommandHandler(AppDbContext context) => _context = context;

    public async Task<Unit> Handle(VerifyPurchaseRequisitionCommand request, CancellationToken cancellationToken)
    {
        var pr = await _context.SPBs
            .FirstOrDefaultAsync(s => s.Doku == request.Doku && s.Hapus == null, cancellationToken);
        if (pr is null)
        {
            throw new NotFoundException($"Purchase Requisition '{request.Doku}' was not found.");
        }
        if (pr.StsVerify == true)
        {
            throw new DomainException($"Purchase Requisition '{request.Doku}' is already verified.");
        }

        _context.Entry(pr).Property(e => e.RowVersion).OriginalValue = request.IfMatchRowVersion;

        pr.StsVerify = true;
        pr.TglVerify = DateTime.UtcNow;

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConcurrencyException(
                $"Purchase Requisition '{request.Doku}' was modified by another user. Please reload and retry.");
        }

        return Unit.Value;
    }
}