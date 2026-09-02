using System.Text.Json.Serialization;

namespace GalvaERP.Features.PurchaseReturns.DTOs;

public record ReturnEligibleLineDto(
    string Doku_Faktur,
    string? Doku_LPB,
    string? NPO,
    string? Kode_Brg,
    string? Alias,
    string? Kode_Gudang,
    double Harga,
    double HPP,
    [property: JsonPropertyName("ppnBm")] double PPnBm,
    double JumlahTersedia);
