namespace GalvaERP.Features.POConfirmations.DTOs;

public record POConfirmationLineDto(
    long id_sub_po_confirmation,
    long? id_sub_po,
    string? Kode_Brg,
    double? Jumlah,
    double? Harga,
    double? Total,
    string? Kode_Gudang,
    string? Note);
