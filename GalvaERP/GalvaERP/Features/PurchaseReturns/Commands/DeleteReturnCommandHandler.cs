using GalvaERP.Common.Exceptions;
using GalvaERP.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GalvaERP.Features.PurchaseReturns.Commands;

public sealed class DeleteReturnCommandHandler : IRequestHandler<DeleteReturnCommand, Unit>
{
    private readonly AppDbContext _context;

    public DeleteReturnCommandHandler(AppDbContext context) => _context = context;

    public async Task<Unit> Handle(DeleteReturnCommand request, CancellationToken cancellationToken)
    {
        var retur = await _context.ReturBelis
            .FirstOrDefaultAsync(document => document.Doku == request.Doku && document.Hapus == null, cancellationToken);
        if (retur is null)
            throw new NotFoundException($"Purchase return '{request.Doku}' not found.");

        UpdateReturnCommandHandler.EnsureEditable(retur, request.Doku);
        _context.Entry(retur).Property(document => document.RowVersion).OriginalValue = request.IfMatchRowVersion;

        var lines = await _context.SubReturBelis
            .Where(line => line.Doku == request.Doku)
            .ToListAsync(cancellationToken);
        _context.SubReturBelis.RemoveRange(lines);
        _context.ReturBelis.Remove(retur);

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConcurrencyException($"Purchase return '{request.Doku}' was modified by another user. Please reload and retry.");
        }

        return Unit.Value;
    }
}
