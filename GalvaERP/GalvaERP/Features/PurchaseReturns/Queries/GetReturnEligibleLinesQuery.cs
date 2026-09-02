using GalvaERP.Features.PurchaseReturns.DTOs;
using MediatR;

namespace GalvaERP.Features.PurchaseReturns.Queries;

public record GetReturnEligibleLinesQuery(string Doku_Faktur) : IRequest<List<ReturnEligibleLineDto>>;
