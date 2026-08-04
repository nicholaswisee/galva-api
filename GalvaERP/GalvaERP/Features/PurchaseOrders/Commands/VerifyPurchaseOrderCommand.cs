using MediatR;

namespace GalvaERP.Features.PurchaseOrders.Commands;

public record VerifyPurchaseOrderCommand(string Doku, byte[] IfMatchRowVersion) : IRequest<Unit>;