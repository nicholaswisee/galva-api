using System.Collections.Generic;
using GalvaERP.Features.PurchaseRequisitions.DTOs;
using MediatR;

namespace GalvaERP.Features.PurchaseRequisitions.Queries;

public record GetPurchaseRequisitionsQuery : IRequest<List<PRListDto>>;
