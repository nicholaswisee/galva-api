namespace GalvaERP.Features.GoodsReceipts.DTOs;

public record GRDetailLineDto(
    string? Kode_Brg,
    double? Jumlah,
    double? Harga,
    double? Nilai,
    string? Kode_Gudang);
