using MediatR;
using System.Text.Json.Serialization;

namespace GalvaERP.Features.PurchaseOrders.Commands;

public record ConfirmPurchaseOrderCommand(
    [property: JsonPropertyName("doku_PO")] string Doku_POSem,
    DateTime Tgl,
    string? ContactPr,
    DateTime? Psd,
    DateTime? Etd,
    string? Memo,
    List<ConfirmedDraftLine> LineItems) : IRequest<string>;

public record ConfirmedDraftLine(
    [property: JsonPropertyName("id_sub_po")] long id_sub_posem,
    string Kode_Brg,
    double Jumlah,
    double Harga,
    string? Kode_Gudang,
    string? Note);
