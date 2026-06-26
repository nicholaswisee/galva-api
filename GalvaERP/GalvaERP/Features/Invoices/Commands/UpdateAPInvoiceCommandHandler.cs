using GalvaERP.Common.Exceptions;
using GalvaERP.Features.Invoices.DTOs;
using GalvaERP.Features.Invoices.Queries;
using GalvaERP.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GalvaERP.Features.Invoices.Commands;

public class UpdateAPInvoiceCommandHandler : IRequestHandler<UpdateAPInvoiceCommand, InvoiceDetailDto>
{
    private readonly AppDbContext _context;

    public UpdateAPInvoiceCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<InvoiceDetailDto> Handle(UpdateAPInvoiceCommand request, CancellationToken cancellationToken)
    {
        var voucher = await _context.VoucherAPs
            .FirstOrDefaultAsync(v => v.Doku == request.Doku, cancellationToken);

        if (voucher is null)
        {
            throw new NotFoundException($"AP invoice '{request.Doku}' not found");
        }

        // Optimistic concurrency: check ETag matches current RowVersion.
        var currentETag = Convert.ToBase64String(voucher.RowVersion);
        if (!string.Equals(currentETag, request.ETag, StringComparison.Ordinal))
        {
            throw new ConcurrencyException(
                $"AP invoice '{request.Doku}' was modified by another user. Please reload and retry.");
        }

        if (request.STS is not null) voucher.STS = request.STS;
        if (request.Status is not null) voucher.Status = request.Status;
        if (request.Keterangan is not null) voucher.Keterangan = request.Keterangan;
        if (request.Kode_Bank is not null) voucher.Kode_Bank = request.Kode_Bank;

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConcurrencyException(
                $"AP invoice '{request.Doku}' was modified by another user. Please reload and retry.");
        }

        // Re-query to get fresh RowVersion.
        var detail = await new GetInvoiceByIdQueryHandler(_context)
            .Handle(new GetInvoiceByIdQuery(request.Doku), cancellationToken);

        return detail;
    }
}
