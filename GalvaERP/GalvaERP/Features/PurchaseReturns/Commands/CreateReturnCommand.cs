using System.Text.Json.Serialization;
using MediatR;

namespace GalvaERP.Features.PurchaseReturns.Commands;

public record CreateReturnLineItem(
    string Doku_Faktur,
    string Doku_LPB,
    string? NPO,
    string Kode_Brg,
    string? Kode_Gudang,
    double Jumlah,
    double Diskon,
    short NoUrut);

public record CreateReturnCommand(
    DateTime Tgl,
    string Doku_Faktur,
    string? Kode_Dept,
    string Kode_Valas,
    double Kurs,
    string? Doku_FP,
    DateTime? Tgl_FP,
    string? Memo,
    [property: JsonPropertyName("ppn")] double PPn,
    string? Type,
    string? TipeRetur,
    List<CreateReturnLineItem> LineItems) : IRequest<string>;
