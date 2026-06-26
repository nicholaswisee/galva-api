namespace GalvaERP.Features.PurchaseOrders.DTOs;

public record POLineItemDto(
    string Kode_Brg,
    double Jumlah,
    double Harga,
    string? Kode_Gudang,
    string? Alias);
