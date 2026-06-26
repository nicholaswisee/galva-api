using GalvaERP.Features.Payments.DTOs;
using MediatR;

namespace GalvaERP.Features.Payments.Commands;

public record CreatePaymentCommand(
    string Kode_Supplier,
    DateTime Tgl,
    string? Kode_BankSupplier,
    string? Keterangan,
    double NilaiKas,
    double NilaiGiro,
    string? Kode_Valas,
    double Kurs,
    List<PaymentLineItemDto> LineItems) : IRequest<string>;
