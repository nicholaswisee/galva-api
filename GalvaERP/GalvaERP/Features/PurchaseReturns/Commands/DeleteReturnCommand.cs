using MediatR;

namespace GalvaERP.Features.PurchaseReturns.Commands;

public record DeleteReturnCommand(string Doku, byte[] IfMatchRowVersion) : IRequest<Unit>;
