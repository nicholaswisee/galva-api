using GalvaERP.Features.Invoices.DTOs;
using MediatR;

namespace GalvaERP.Features.Invoices.Queries;

public record GetInvoiceByIdQuery(string Doku) : IRequest<InvoiceDetailDto>;
