using MediatR;

namespace GalvaERP.Features.Invoices.Commands;

public record DeleteAPInvoiceCommand(
    string Doku,
    byte[] IfMatchRowVersion) : IRequest<Unit>;
