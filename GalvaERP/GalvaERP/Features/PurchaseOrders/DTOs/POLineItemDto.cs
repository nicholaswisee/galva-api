namespace GalvaERP.Features.PurchaseOrders.DTOs;

public record POLineItemDto(
    string Kode_Brg,
    string? Merk,
    string? Model,
    string? Satuan,
    double Jumlah,
    double Harga,
    double DiscPct,
    double Disc,
    double Total,
    string? Kode_Gudang,
    string? Alias,
    string? Note,
    string? Schedule,
    string? Kode_Valas,
    double? Kurs,
    double? Ppn);