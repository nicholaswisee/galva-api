using GalvaERP.Features.PurchaseRequisitions.DTOs;
using MediatR;

namespace GalvaERP.Features.PurchaseRequisitions.Commands;

public record UpdatePurchaseRequisitionCommand(
    string Doku,
    string? Status,
    byte[] IfMatchRowVersion) : IRequest<PRDetailDto>;
