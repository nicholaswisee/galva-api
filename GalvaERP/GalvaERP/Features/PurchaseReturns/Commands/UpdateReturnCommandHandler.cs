using GalvaERP.Common.Exceptions;
using GalvaERP.Features.PurchaseReturns.DTOs;
using GalvaERP.Features.PurchaseReturns.Queries;
using GalvaERP.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GalvaERP.Features.PurchaseReturns.Commands;

public sealed class UpdateReturnCommandHandler : IRequestHandler<UpdateReturnCommand, ReturnDetailDto>
{
    private readonly AppDbContext _context;

    public UpdateReturnCommandHandler(AppDbContext context) => _context = context;

    public async Task<ReturnDetailDto> Handle(UpdateReturnCommand request, CancellationToken cancellationToken)
    {
        var retur = await _context.ReturBelis
            .FirstOrDefaultAsync(document => document.Doku == request.Doku && document.Hapus == null, cancellationToken);
        if (retur is null)
            throw new NotFoundException($"Purchase return '{request.Doku}' not found.");

        EnsureEditable(retur, request.Doku);
        _context.Entry(retur).Property(document => document.RowVersion).OriginalValue = request.IfMatchRowVersion;
        retur.STS = request.STS;
        retur.MEMO = request.Memo;
        retur.Validasi = request.Validasi;
        retur.StatusGL = request.StatusGL;

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConcurrencyException($"Purchase return '{request.Doku}' was modified by another user. Please reload and retry.");
        }

        return await new GetReturnByIdQueryHandler(_context)
            .Handle(new GetReturnByIdQuery(request.Doku), cancellationToken);
    }

    internal static void EnsureEditable(Domain.Entities.ReturBeli retur, string doku)
    {
        if (retur.Validasi == true || retur.SyncToCMG == true || retur.STS is "1" or "9")
            throw new DomainException($"Purchase return '{doku}' cannot be changed because it is validated, synchronized, or posted.");
    }
}
