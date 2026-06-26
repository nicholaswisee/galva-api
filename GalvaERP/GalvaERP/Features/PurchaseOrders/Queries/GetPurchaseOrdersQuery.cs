using System.Collections.Generic;
using GalvaERP.Features.PurchaseOrders.DTOs;
using MediatR;

namespace GalvaERP.Features.PurchaseOrders.Queries;

public record GetPurchaseOrdersQuery : IRequest<List<POListDto>>;
