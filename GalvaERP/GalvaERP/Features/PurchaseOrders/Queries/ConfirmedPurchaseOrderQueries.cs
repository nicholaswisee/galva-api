using GalvaERP.Features.PurchaseOrders.DTOs;
using MediatR;

namespace GalvaERP.Features.PurchaseOrders.Queries;

public record GetConfirmedPurchaseOrdersQuery : IRequest<List<ConfirmedPurchaseOrderListDto>>;

public record GetConfirmedPurchaseOrderByIdQuery(string Doku) : IRequest<ConfirmedPurchaseOrderDetailDto?>;
