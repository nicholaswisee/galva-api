using System;
using System.Collections.Generic;
using GalvaERP.Features.PurchaseRequisitions.DTOs;
using MediatR;

namespace GalvaERP.Features.PurchaseRequisitions.Commands;

public record CreatePurchaseRequisitionCommand(
    string Kode_Dept,
    DateTime Tgl,
    string? Kode_Sales,
    string? Memo,
    List<PRLineItemDto> LineItems) : IRequest<string>;
