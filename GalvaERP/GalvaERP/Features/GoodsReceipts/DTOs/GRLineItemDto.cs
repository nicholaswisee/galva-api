namespace GalvaERP.Features.GoodsReceipts.DTOs;

public record GRLineItemDto(
    string Kode_Brg,
    double Jumlah,
    double Harga,
    string? Kode_Gudang,
    long id_sub_po_confirmation);
