namespace GalvaERP.Features.PurchaseOrders.DTOs;

public record PODetailLineDto(
    long id_sub_po,
    string? Kode_Brg,
    string? Merk,
    string? Model,
    string? Satuan,
    double? Jumlah,
    double? Harga,
    double? DiscPct,
    double? Disc,
    double? Total,
    double? JumlahKonfirm,
    string? Kode_Gudang,
    string? Alias,
    string? Note,
    string? Schedule);