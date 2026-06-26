using GalvaERP.Features.GoodsReceipts.DTOs;
using MediatR;

namespace GalvaERP.Features.GoodsReceipts.Queries;

public record GetGoodsReceiptsQuery() : IRequest<List<GRListDto>>;
