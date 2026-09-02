using GalvaERP.Common.Exceptions;
using GalvaERP.Features.PurchaseReturns.DTOs;
using GalvaERP.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GalvaERP.Features.PurchaseReturns.Queries;

public sealed class GetReturnEligibleLinesQueryHandler
    : IRequestHandler<GetReturnEligibleLinesQuery, List<ReturnEligibleLineDto>>
{
    private readonly AppDbContext _context;

    public GetReturnEligibleLinesQueryHandler(AppDbContext context) => _context = context;

    public async Task<List<ReturnEligibleLineDto>> Handle(
        GetReturnEligibleLinesQuery request,
        CancellationToken cancellationToken)
    {
        if (!await _context.VoucherAPs.AnyAsync(invoice => invoice.Doku == request.Doku_Faktur, cancellationToken))
            throw new NotFoundException($"Source invoice {request.Doku_Faktur} does not exist");

        var lines = await ReturnSourceLines.LoadAsync(_context, request.Doku_Faktur, cancellationToken);
        return lines
            .Where(line => line.JumlahTersedia > 0.0)
            .Select(line => new ReturnEligibleLineDto(
                request.Doku_Faktur,
                line.Doku_LPB,
                line.NPO,
                line.Kode_Brg,
                line.Alias,
                line.Kode_Gudang,
                line.Harga,
                line.HPP,
                line.PPnBm,
                line.JumlahTersedia))
            .ToList();
    }
}
