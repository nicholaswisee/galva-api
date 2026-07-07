namespace GalvaERP.Features.PurchaseOrders.DTOs;

public record POConfirmLineItemDto(
    long id_sub_po,
    string Kode_Brg,
    double Jumlah,
    double Harga,
    string? Note);