namespace GalvaERP.Features.Payments.DTOs;

public record PaymentLineItemDto(
    string Doku_LPB,
    double Nilai,
    double TotalNilai);