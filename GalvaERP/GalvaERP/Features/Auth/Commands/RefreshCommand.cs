using GalvaERP.Features.Auth.DTOs;
using MediatR;

namespace GalvaERP.Features.Auth.Commands;

public record RefreshCommand(string RefreshToken) : IRequest<LoginResult>;
