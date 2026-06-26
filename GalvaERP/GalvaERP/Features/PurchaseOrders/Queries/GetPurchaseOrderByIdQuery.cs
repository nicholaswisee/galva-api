using GalvaERP.Features.PurchaseOrders.DTOs;
using MediatR;

namespace GalvaERP.Features.PurchaseOrders.Queries;

public record GetPurchaseOrderByIdQuery(string Doku) : IRequest<PODetailDto?>;
