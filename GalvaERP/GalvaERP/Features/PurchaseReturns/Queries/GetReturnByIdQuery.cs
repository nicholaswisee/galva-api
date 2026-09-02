using GalvaERP.Features.PurchaseReturns.DTOs;
using MediatR;

namespace GalvaERP.Features.PurchaseReturns.Queries;

public record GetReturnByIdQuery(string Doku) : IRequest<ReturnDetailDto>;
