using MediatR;

namespace GalvaERP.Features.PurchaseRequisitions.Commands;

public record VerifyPurchaseRequisitionCommand(string Doku, byte[] IfMatchRowVersion) : IRequest<Unit>;