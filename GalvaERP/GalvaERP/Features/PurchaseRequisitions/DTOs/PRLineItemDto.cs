namespace GalvaERP.Features.PurchaseRequisitions.DTOs;

public record PRLineItemDto(
    string Kode_Brg,
    double Jumlah,
    double Harga,
    string? Kode_Gudang,
    string? Alias);
