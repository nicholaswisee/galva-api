using System;
using System.Collections.Generic;
using GalvaERP.Features.PurchaseOrders.DTOs;
using MediatR;

namespace GalvaERP.Features.PurchaseOrders.Commands;

public record CreatePurchaseOrderCommand(
    string Kode_Supplier,
    string Kode_dept,
    DateTime Tgl,
    string? Memo,
    string Kode_Valas,
    double Kurs,
    double Ppn,
    double Diskon,
    double DppNilaiLain,
    double PPnTunai,
    short Syarat,
    List<POLineItemDto> LineItems) : IRequest<string>;
