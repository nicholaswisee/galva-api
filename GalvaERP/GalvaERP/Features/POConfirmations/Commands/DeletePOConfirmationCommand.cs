using MediatR;

namespace GalvaERP.Features.POConfirmations.Commands;

public record DeletePOConfirmationCommand(
    string Doku,
    byte[] IfMatchRowVersion) : IRequest<Unit>;
