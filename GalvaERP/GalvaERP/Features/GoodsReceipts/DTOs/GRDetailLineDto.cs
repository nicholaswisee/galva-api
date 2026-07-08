namespace GalvaERP.Features.GoodsReceipts.DTOs;

public record GRDetailLineDto(
    long id_sub_po_confirmation,
    string? Kode_Brg,
    double? Jumlah,
    double? Harga,
    double? Nilai,
    string? Kode_Gudang);
