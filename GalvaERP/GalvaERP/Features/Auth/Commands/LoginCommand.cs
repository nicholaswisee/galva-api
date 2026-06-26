using GalvaERP.Features.Auth.DTOs;
using MediatR;

namespace GalvaERP.Features.Auth.Commands;

public record LoginCommand(string Username, string Password) : IRequest<LoginResult>;
