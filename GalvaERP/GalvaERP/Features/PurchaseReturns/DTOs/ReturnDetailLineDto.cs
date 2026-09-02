using System.Text.Json.Serialization;

namespace GalvaERP.Features.PurchaseReturns.DTOs;

public record ReturnDetailLineDto(
    long PKbas,
    string? Doku_Faktur,
    string? Doku_LPB,
    string? NPO,
    string? Kode_Brg,
    string? Kode_Gudang,
    string? Alias,
    double Jumlah,
    double Harga,
    double Diskon,
    double PPN,
    [property: JsonPropertyName("ppnBm")] double PPnBm,
    double HPP,
    double Nilai,
    short NoUrut);
