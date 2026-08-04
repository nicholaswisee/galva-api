using GalvaERP.Common.Exceptions;
using GalvaERP.Features.Payments.DTOs;
using GalvaERP.Features.Payments.Queries;
using GalvaERP.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GalvaERP.Features.Payments.Commands;

public class UpdatePaymentCommandHandler : IRequestHandler<UpdatePaymentCommand, PaymentDetailDto>
{
    private readonly AppDbContext _context;

    public UpdatePaymentCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PaymentDetailDto> Handle(UpdatePaymentCommand request, CancellationToken cancellationToken)
    {
        var bayar = await _context.Bayars
            .FirstOrDefaultAsync(b => b.Doku == request.Doku && b.Hapus == null, cancellationToken);
        if (bayar is null)
        {
            throw new NotFoundException($"Payment '{request.Doku}' not found");
        }

        // Optimistic concurrency via the If-Match header (canonical pattern).
        // Set the original RowVersion so EF Core emits the concurrency check on SaveChanges.
        _context.Entry(bayar).Property(e => e.RowVersion).OriginalValue = request.IfMatchRowVersion;

        if (request.STS is not null) bayar.STS = request.STS;
        if (request.Keterangan is not null) bayar.Keterangan = request.Keterangan;
        if (request.Kode_BankSupplier is not null) bayar.Kode_BankSupplier = request.Kode_BankSupplier;
        if (request.NilaiKas.HasValue) bayar.NilaiKas = request.NilaiKas;
        if (request.NilaiGiro.HasValue) bayar.NilaiGiro = request.NilaiGiro;

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConcurrencyException(
                $"Payment '{request.Doku}' was modified by another user. Please reload and retry.");
        }

        // Re-query to get the fresh RowVersion / ETag and consistent DTO.
        var detail = await new GetPaymentByIdQueryHandler(_context)
            .Handle(new GetPaymentByIdQuery(request.Doku), cancellationToken);

        return detail;
    }
}