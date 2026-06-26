using GalvaERP.Features.PurchaseRequisitions.DTOs;
using MediatR;

namespace GalvaERP.Features.PurchaseRequisitions.Queries;

public record GetPurchaseRequisitionByIdQuery(string Doku) : IRequest<PRDetailDto?>;
