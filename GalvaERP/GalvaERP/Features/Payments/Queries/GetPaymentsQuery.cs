using GalvaERP.Features.Payments.DTOs;
using MediatR;

namespace GalvaERP.Features.Payments.Queries;

public record GetPaymentsQuery() : IRequest<List<PaymentListDto>>;
