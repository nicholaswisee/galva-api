using GalvaERP.Features.GoodsReceipts.DTOs;
using MediatR;

namespace GalvaERP.Features.GoodsReceipts.Commands;

public record CreateGoodsReceiptCommand(
    string Doku_PO,
    DateTime Tgl,
    string? Kode_Supplier,
    string? SuratJalan,
    string? Memo,
    List<GRLineItemDto> LineItems) : IRequest<string>;
