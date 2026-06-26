namespace GalvaERP.Features.Payments.DTOs;

public record PaymentLineItemDto(
    string Doku_LPB,
    string? Doku_PO,
    string? Doku_Voucher,
    double Nilai,
    double NilaiBayar);
