using GalvaERP.Features.PurchaseReturns.DTOs;
using MediatR;

namespace GalvaERP.Features.PurchaseReturns.Queries;

public record GetReturnsQuery : IRequest<List<ReturnListDto>>;
