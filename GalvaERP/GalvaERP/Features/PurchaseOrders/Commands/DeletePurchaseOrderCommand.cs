using MediatR;

namespace GalvaERP.Features.PurchaseOrders.Commands;

public record DeletePurchaseOrderCommand(
    string Doku,
    byte[] IfMatchRowVersion) : IRequest<Unit>;
