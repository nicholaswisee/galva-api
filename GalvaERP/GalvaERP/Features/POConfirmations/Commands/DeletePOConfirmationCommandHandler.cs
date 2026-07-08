using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GalvaERP.Common.Exceptions;
using GalvaERP.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GalvaERP.Features.POConfirmations.Commands;

public class DeletePOConfirmationCommandHandler : IRequestHandler<DeletePOConfirmationCommand, Unit>
{
    private readonly AppDbContext _context;

    public DeletePOConfirmationCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeletePOConfirmationCommand request, CancellationToken cancellationToken)
    {
        var confirmation = await _context.POConfirmations
            .FirstOrDefaultAsync(p => p.Doku == request.Doku, cancellationToken);

        if (confirmation is null)
        {
            throw new NotFoundException($"PO Confirmation '{request.Doku}' was not found.");
        }

        if (confirmation.STS == "9")
        {
            throw new NotFoundException($"PO Confirmation '{request.Doku}' was not found.");
        }

        if (!confirmation.RowVersion.SequenceEqual(request.IfMatchRowVersion))
        {
            throw new ConcurrencyException(
                $"PO Confirmation '{request.Doku}' was modified by another user. Please reload and try again.");
        }

        var po = await _context.POs
            .FirstOrDefaultAsync(p => p.Doku == confirmation.Doku_PO, cancellationToken);

        if (po is null)
        {
            throw new NotFoundException($"Purchase Order '{confirmation.Doku_PO}' was not found.");
        }

        if (po.STS != "0")
        {
            throw new DomainException(
                $"PO Confirmation '{request.Doku}' can only be deleted while parent PO STS is '0' (Pending). Current parent STS: {po.STS}.");
        }

        // Reverse quantities on parent SubPOs.
        var subConfirmationLines = await _context.SubPOConfirmations
            .Where(sc => sc.Doku == request.Doku)
            .ToListAsync(cancellationToken);

        var subPOIds = subConfirmationLines
            .Select(sc => sc.id_sub_po ?? 0L)
            .Where(id => id != 0L)
            .Distinct()
            .ToList();

        var subPOs = await _context.SubPOs
            .Where(sp => sp.Doku == confirmation.Doku_PO && subPOIds.Contains(sp.id_sub_po))
            .ToListAsync(cancellationToken);

        var subPOById = subPOs.ToDictionary(s => s.id_sub_po);

        foreach (var line in subConfirmationLines)
        {
            var idSubPo = line.id_sub_po ?? 0;
            if (idSubPo == 0) continue;
            if (!subPOById.TryGetValue(idSubPo, out var subPO)) continue;

            subPO.JumlahKonfirm = (subPO.JumlahKonfirm ?? 0) - (line.Jumlah ?? 0);
        }

        // Soft-delete: mark confirmation as cancelled. SubPOConfirmation children are removed since
        // the parent confirmation is no longer live and JumlahKonfirm has been reversed.
        confirmation.STS = "9";
        _context.SubPOConfirmations.RemoveRange(subConfirmationLines);

        // Recompute parent PO STS based on all SubPOs.
        var allSubPOs = await _context.SubPOs
            .Where(s => s.Doku == confirmation.Doku_PO)
            .ToListAsync(cancellationToken);
        var allFullyConfirmed = allSubPOs.All(s => (s.JumlahKonfirm ?? 0) >= (s.Jumlah ?? 0) - 0.0001);
        po.STS = allFullyConfirmed ? "1" : "0";

        _context.Entry(confirmation).Property(p => p.RowVersion).OriginalValue = request.IfMatchRowVersion;

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConcurrencyException(
                $"PO Confirmation '{request.Doku}' was modified by another user. Please reload and try again.");
        }

        return Unit.Value;
    }
}
