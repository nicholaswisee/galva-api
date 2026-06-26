using GalvaERP.Features.Payments.DTOs;
using MediatR;

namespace GalvaERP.Features.Payments.Queries;

public record GetPaymentByIdQuery(string Doku) : IRequest<PaymentDetailDto>;
