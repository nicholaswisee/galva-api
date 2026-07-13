using GalvaERP.Common.Exceptions;
using GalvaERP.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GalvaERP.Features.Invoices.Commands;

public class DeleteAPInvoiceCommandHandler : IRequestHandler<DeleteAPInvoiceCommand, Unit>
{
    private readonly AppDbContext _context;

    public DeleteAPInvoiceCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteAPInvoiceCommand request, CancellationToken cancellationToken)
    {
        var invoice = await _context.VoucherAPs
            .FirstOrDefaultAsync(v => v.Doku == request.Doku, cancellationToken);

        if (invoice is null)
        {
            throw new NotFoundException($"AP invoice '{request.Doku}' was not found.");
        }

        if (invoice.STS != "0")
        {
            throw new DomainException(
                $"AP invoice '{request.Doku}' can only be deleted while STS is '0' (Pending).");
        }

        _context.Entry(invoice).Property(v => v.RowVersion).OriginalValue = request.IfMatchRowVersion;

        var subRows = await _context.SubVoucherAPs
            .Where(s => s.Doku == request.Doku)
            .ToListAsync(cancellationToken);

        _context.SubVoucherAPs.RemoveRange(subRows);
        _context.VoucherAPs.Remove(invoice);

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConcurrencyException(
                $"AP invoice '{request.Doku}' was modified by another user. Please reload and try again.");
        }

        return Unit.Value;
    }
}
