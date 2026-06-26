using GalvaERP.Features.GoodsReceipts.DTOs;
using MediatR;

namespace GalvaERP.Features.GoodsReceipts.Queries;

public record GetGoodsReceiptByIdQuery(string Doku) : IRequest<GRDetailDto>;
