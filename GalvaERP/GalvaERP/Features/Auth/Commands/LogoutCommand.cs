using MediatR;

namespace GalvaERP.Features.Auth.Commands;

public record LogoutCommand(int UserId) : IRequest;
