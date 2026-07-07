using System;
using System.Collections.Generic;
using GalvaERP.Features.PurchaseOrders.DTOs;
using MediatR;

namespace GalvaERP.Features.PurchaseOrders.Commands;

public record ConfirmPurchaseOrderCommand(
    string Doku,
    byte[] IfMatchRowVersion,
    DateTime Tgl,
    string? ContactPr,
    DateTime? Psd,
    DateTime? Etd,
    string? Memo,
    List<POConfirmLineItemDto> LineItems) : IRequest<PODetailDto>;
