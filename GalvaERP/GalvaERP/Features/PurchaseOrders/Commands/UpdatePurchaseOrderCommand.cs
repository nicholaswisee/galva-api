using GalvaERP.Features.PurchaseOrders.DTOs;
using MediatR;

namespace GalvaERP.Features.PurchaseOrders.Commands;

public record UpdatePurchaseOrderCommand(
    string Doku,
    string? STS,
    byte[] IfMatchRowVersion) : IRequest<PODetailDto>;
