using MediatR;

namespace GalvaERP.Features.GoodsReceipts.Commands;

public record DeleteGoodsReceiptCommand(
    string Doku,
    byte[] IfMatchRowVersion) : IRequest<Unit>;
