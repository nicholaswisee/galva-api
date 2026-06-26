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
            .FirstOrDefaultAsync(b => b.Doku == request.Doku, cancellationToken);

        if (bayar is null)
        {
            throw new NotFoundException($"Payment '{request.Doku}' not found");
        }

        // Optimistic concurrency: check ETag matches current RowVersion.
        var currentETag = Convert.ToBase64String(bayar.RowVersion);
        if (!string.Equals(currentETag, request.ETag, StringComparison.Ordinal))
        {
            throw new ConcurrencyException(
                $"Payment '{request.Doku}' was modified by another user. Please reload and retry.");
        }

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

        // Re-query to get fresh RowVersion.
        var detail = await new GetPaymentByIdQueryHandler(_context)
            .Handle(new GetPaymentByIdQuery(request.Doku), cancellationToken);

        return detail;
    }
}
