using GalvaERP.Features.Payments.DTOs;
using MediatR;

namespace GalvaERP.Features.Payments.Commands;

public record UpdatePaymentCommand(
    string Doku,
    string? STS,
    string? Keterangan,
    string? Kode_BankSupplier,
    double? NilaiKas,
    double? NilaiGiro,
    string ETag) : IRequest<PaymentDetailDto>;
