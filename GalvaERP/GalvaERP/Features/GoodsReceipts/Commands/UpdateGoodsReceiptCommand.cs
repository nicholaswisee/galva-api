using GalvaERP.Features.GoodsReceipts.DTOs;
using MediatR;

namespace GalvaERP.Features.GoodsReceipts.Commands;

public record UpdateGoodsReceiptCommand(
    string Doku,
    string? STS,
    string? Status,
    string? Memo,
    double? PPN,
    string ETag) : IRequest<GRDetailDto>;
